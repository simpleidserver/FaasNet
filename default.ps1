properties {
	$base_dir = resolve-path .
	$build_dir = "$base_dir\build"
	$source_dir = "$base_dir\src"
	$result_dir = "$build_dir\results"
	$global:config = "debug"
	$tag = $(git tag -l --points-at HEAD)
	$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
	$suffix = @{ $true = ""; $false = "ci-$revision"}[$tag -ne $NULL -and $revision -ne "local"]
	$commitHash = $(git rev-parse --short HEAD)
	$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]
    $versionSuffix = @{ $true = "--version-suffix=$($suffix)"; $false = ""}[$suffix -ne ""]
}


task default -depends local
task local -depends compile, test
task ci -depends clean, release, local, pack, publish

task publish {
	exec { dotnet publish $source_dir\FaasNet.Runtime.Startup\FaasNet.Runtime.Startup.csproj -c $config -o $result_dir\services\Runtime }
	exec { dotnet publish $source_dir\FaasNet.Runtime.GetSql\FaasNet.Runtime.GetSql.csproj -c $config -o $result_dir\services\RuntimeGetSql }
	exec { dotnet publish $source_dir\FaasNet.Runtime.Transform\FaasNet.Runtime.Transform.csproj -c $config -o $result_dir\services\RuntimeTransform }
	exec { dotnet publish $source_dir\FaasNet.Kubernetes\FaasNet.Kubernetes.csproj -c $config -o $result_dir\services\Kubernetes }
	exec { dotnet publish $source_dir\FaasNet.Gateway.Startup\FaasNet.Gateway.Startup.csproj -c $config -o $result_dir\services\Gateway }
	exec { dotnet publish $source_dir\FaasNet.CLI\FaasNet.CLI.csproj -c $config -o $result_dir\cli }
}

task clean {
	rd "$source_dir\artifacts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	rd "$base_dir\build" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task release {
    $global:config = "release"
}

task compile -depends clean {
	echo "build: Tag is $tag"
	echo "build: Package version suffix is $suffix"
	echo "build: Build version suffix is $buildSuffix" 
	
	exec { dotnet --version }
	exec { dotnet --info }

	exec { msbuild -version }
	
    exec { dotnet build .\FaasNet.sln -c $config --version-suffix=$buildSuffix }
}

task publishHelmAndWebsite {
	exec { git checkout gh-pages }
	exec { Get-ChildItem -Path . -Include *.* -File -Recurse | foreach { $_.Delete()} }
	exec { git add . }
	exec { git commit -m "Remove" }
	exec { git checkout master }
	exec { docfx ./docs/docfx.json }
	rd "$base_dir\docs\charts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	exec { helm package ./helm -d ./docs/charts }
	exec { Copy-item "./helm/Chart.yaml" -Destination "./docs/charts" }
	exec { helm repo index ./docs/charts }
	exec { Remove-Item "./docs/charts/Chart.yaml" }
	exec { Copy-item -Force -Recurse -Verbose "./docs/_site/*" -Destination "." }
	exec { git checkout gh-pages --merge }
	exec { git add . }
	exec { git commit -m "Update Documentation" }
	exec { git rebase -i HEAD~2 }
	exec { git push origin gh-pages }
	exec { git checkout master }
}

task buildLocalDockerImage -depends publish {
	exec { docker build -f RuntimeGetSqlDockerfile -t localhost:5000/faasgetsql . }
	exec { docker build -f RuntimeGetSqlDockerfile -t localhost:5000/faastransform . }
	exec { docker build -f KubernetesDockerfile -t localhost:5000/faaskubernetes . }
	exec { docker build -f GatewayDockerfile -t localhost:5000/faasgateway . }
	exec { docker push localhost:5000/faasgetsql }
	exec { docker push localhost:5000/faastransform }
	exec { docker push localhost:5000/faaskubernetes }
	exec { docker push localhost:5000/faasgateway }
}

task initLocalKubernetes {
	exec { kubectl apply -f ./kubernetes/faas-namespace.yml }
    exec { kubectl apply -f ./kubernetes/run-mssql.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/mssql-external-svc.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/mssql-internal-svc.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/run-faas-kubernetes.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-kubernetes-svc.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/run-faas-gateway.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-gateway-svc.yml --namespace=faas }
}

task pack -depends release, compile {
	exec { dotnet pack $source_dir\FaasNet.Runtime\FaasNet.Runtime.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\FaasNet.Templates\FaasNet.Templates.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task test {
}