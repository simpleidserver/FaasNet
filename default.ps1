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
	exec { dotnet publish $source_dir\FaasNet.Function.GetSql\FaasNet.Function.GetSql.csproj -c $config -o $result_dir\services\RuntimeGetSql }
	exec { dotnet publish $source_dir\FaasNet.Function.Transform\FaasNet.Function.Transform.csproj -c $config -o $result_dir\services\RuntimeTransform }
	exec { dotnet publish $source_dir\FaasNet.Kubernetes\FaasNet.Kubernetes.csproj -c $config -o $result_dir\services\Kubernetes }
	# exec { dotnet publish $source_dir\FaasNet.Gateway.Startup\FaasNet.Gateway.Startup.csproj -c $config -o $result_dir\services\Gateway }
	# exec { dotnet publish $source_dir\FaasNet.CLI\FaasNet.CLI.csproj -c $config -o $result_dir\cli }
}

task publishTemplate {
	exec { dotnet pack $source_dir\FaasNet.Templates\FaasNet.Templates.csproj -c $config --no-build --output $result_dir }
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
	exec { git rm -r . }
	exec { git checkout HEAD -- .gitignore }
	exec { git add . }
	exec { git commit -m "Remove" }
	exec { git checkout master }
	exec { helm package ./helm -d ./docs/charts }
	exec { Copy-item "./helm/Chart.yaml" -Destination "./docs/charts" }
	exec { helm repo index ./docs/charts }
	exec { Remove-Item "./docs/charts/Chart.yaml" }
	rd "$base_dir\docs\obj" -recurse -force  -ErrorAction SilentlyContinue | out-null
	exec { docfx ./docs/docfx.json }
	exec { git add . }
	exec { git commit -m "Publish helm" }
	exec { Copy-item -Force -Recurse -Verbose "./docs/_site/*" -Destination "." }
	exec { git checkout gh-pages --merge }
	exec { git add . }
	exec { git commit -m "Update Documentation" }
	exec { git rebase -i HEAD~2 }
	exec { git push origin gh-pages }
	exec { git checkout master }
}

task buildLocalDockerImage -depends publish {
	exec { npm run docker --prefix $source_dir\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t localhost:5000/faasgetsql . }
	exec { docker build -f RuntimeTransformDockerfile -t localhost:5000/faastransform . }
	exec { docker build -f KubernetesDockerfile -t localhost:5000/faaskubernetes . }
	exec { docker build -f GatewayDockerfile -t localhost:5000/faasgateway . }
	exec { docker build -f WebsiteDockerfile -t localhost:5000/faaswebsite . }
	exec { docker build -f PrometheusDockerfile -t localhost:5000/faasprometheus . }
	exec { docker push localhost:5000/faasgetsql }
	exec { docker push localhost:5000/faastransform }
	exec { docker push localhost:5000/faaskubernetes }
	# exec { docker push localhost:5000/faasgateway }
	# exec { docker push localhost:5000/faaswebsite }
	# exec { docker push localhost:5000/faasprometheus }
}

task initLocalKubernetes {
	exec { kubectl apply -f ./kubernetes/faas-namespace.yml }
    exec { kubectl apply -f ./kubernetes/run-mssql.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/mssql-external-svc.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/mssql-internal-svc.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/prometheus-persistent-volume.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/prometheus-persistent-volume-claim.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/run-faas-kubernetes.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-kubernetes-svc.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-kubernetes-external-svc.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/run-faas-gateway.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/faas-gateway-svc.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/run-website.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/faas-website-svc.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/run-prometheus.yml --namespace=faas }
	# exec { kubectl apply -f ./kubernetes/faas-prometheus-svc.yml --namespace=faas }
}

task initDevKubernetes {
	exec { kubectl apply -f ./kubernetes/faas-namespace.yml }
    exec { kubectl apply -f ./kubernetes/prometheus-persistent-volume.yml --namespace=faas }
    exec { kubectl apply -f ./kubernetes/prometheus-persistent-volume-claim.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/run-prometheus.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-prometheus-svc.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/run-faas-kubernetes.yml --namespace=faas }
	exec { kubectl apply -f ./kubernetes/faas-kubernetes-external-svc.yml --namespace=faas }
	
}

task builderDockerImage -depends publish {
	exec { npm run docker --prefix $source_dir\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t simpleidserver/faasgetsql:0.0.3 . }
	exec { docker build -f RuntimeTransformDockerfile -t simpleidserver/faastransform:0.0.3 . }
	exec { docker build -f KubernetesDockerfile -t simpleidserver/faaskubernetes:0.0.3 . }
	exec { docker build -f GatewayDockerfile -t simpleidserver/faasgateway:0.0.3 . }
	exec { docker build -f WebsiteDockerfile -t simpleidserver/faaswebsite:0.0.3 . }
	exec { docker build -f PrometheusDockerfile -t simpleidserver/faasprometheus:0.0.3 . }
	exec { docker push simpleidserver/faasgetsql:0.0.3 }
	exec { docker push simpleidserver/faastransform:0.0.3 }
	exec { docker push simpleidserver/faaskubernetes:0.0.3 }
	exec { docker push simpleidserver/faasgateway:0.0.3 }
	exec { docker push simpleidserver/faaswebsite:0.0.3 }
	exec { docker push simpleidserver/faasprometheus:0.0.3 }
}

task pack -depends release, compile {
	exec { dotnet pack $source_dir\FaasNet.Runtime\FaasNet.Runtime.csproj -c $config --no-build $versionSuffix --output $result_dir }
	# exec { dotnet pack $source_dir\FaasNet.Templates\FaasNet.Templates.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task test {
}