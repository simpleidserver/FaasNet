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

# DEV tasks
task packDev -depends clean, packNoSuffix, packTemplate
task dockerDev -depends clean, publishDockerDev

# CI tasks
task dockerCI -depends clean, publishDockerCI
task ci -depends clean, test, pack, publishCLI

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
    exec { dotnet build .\EventMesh.sln -c $config --version-suffix=$buildSuffix }
}

# Publish assets
task publishCLI {
	exec { dotnet publish $source_dir\FaasNet.CLI\FaasNet.CLI.csproj -c $config -o $result_dir\cli }
}

task publishDocker {
	exec { dotnet publish $source_dir\FaasNet.Function.GetSql\FaasNet.Function.GetSql.csproj -c $config -o $result_dir\services\RuntimeGetSql }
	exec { dotnet publish $source_dir\FaasNet.Function.Transform\FaasNet.Function.Transform.csproj -c $config -o $result_dir\services\RuntimeTransform }
	exec { dotnet publish $source_dir\FaasNet.Kubernetes\FaasNet.Kubernetes.csproj -c $config -o $result_dir\services\Kubernetes }
	exec { dotnet publish $source_dir\FaasNet.Gateway.SqlServer.Startup\FaasNet.Gateway.SqlServer.Startup.csproj -c $config -o $result_dir\services\Gateway }
	exec { dotnet publish $source_dir\EventMesh.Runtime.Website\EventMesh.Runtime.Website.csproj -c $config -o $result_dir\services\EventMeshServer }
}

# Pack
task pack -depends release, compile {
	exec { dotnet pack $source_dir\FaasNet.Function\FaasNet.Function.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\FaasNet.Runtime\FaasNet.Runtime.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime\EventMesh.Runtime.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.AMQP\EventMesh.Runtime.AMQP.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.EF\EventMesh.Runtime.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.Kafka\EventMesh.Runtime.Kafka.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task packNoSuffix -depends release, compile {
	exec { dotnet pack $source_dir\FaasNet.Function\FaasNet.Function.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\FaasNet.Runtime\FaasNet.Runtime.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime\EventMesh.Runtime.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.AMQP\EventMesh.Runtime.AMQP.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.EF\EventMesh.Runtime.EF.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh.Runtime.Kafka\EventMesh.Runtime.Kafka.csproj -c $config --output $result_dir }
}

task packTemplate {
	exec { dotnet pack $source_dir\FaasNet.Templates\FaasNet.Templates.csproj -c $config --no-build --output $result_dir }
}

# Docker
task buildDockerDev -depends publishDocker {
	exec { npm run docker --prefix $source_dir\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t localhost:5000/faasgetsql . }
	exec { docker build -f RuntimeTransformDockerfile -t localhost:5000/faastransform . }
	exec { docker build -f KubernetesDockerfile -t localhost:5000/faaskubernetes . }
	exec { docker build -f GatewayDockerfile -t localhost:5000/faasgateway . }
	exec { docker build -f WebsiteDockerfile -t localhost:5000/faaswebsite . }
	exec { docker build -f PrometheusDockerfile -t localhost:5000/faasprometheus . }
	exec { docker build -f EventMeshDockerFile -t localhost:5000/eventmeshserver . }
}

task buildDockerCI -depends publishDocker {
	exec { npm run docker --prefix $source_dir\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t simpleidserver/faasgetsql:0.0.5 . }
	exec { docker build -f RuntimeTransformDockerfile -t simpleidserver/faastransform:0.0.5 . }
	exec { docker build -f KubernetesDockerfile -t simpleidserver/faaskubernetes:0.0.5 . }
	exec { docker build -f GatewayDockerfile -t simpleidserver/faasgateway:0.0.5 . }
	exec { docker build -f WebsiteDockerfile -t simpleidserver/faaswebsite:0.0.5 . }
	exec { docker build -f PrometheusDockerfile -t simpleidserver/faasprometheus:0.0.5 . }
	exec { docker build -f EventMeshDockerFile -t simpleidserver/eventmeshserver:0.0.5 . }
}

task publishDockerDev -depends buildDockerDev {
	exec { docker push localhost:5000/faasgetsql }
	exec { docker push localhost:5000/faastransform }
	exec { docker push localhost:5000/faaskubernetes }
	exec { docker push localhost:5000/faasgateway }
	exec { docker push localhost:5000/faaswebsite }
	exec { docker push localhost:5000/faasprometheus }
	exec { docker push localhost:5000/eventmeshserver }
}

task publishDockerCI -depends publishDocker {
	exec { docker push simpleidserver/faasgetsql:0.0.5 }
	exec { docker push simpleidserver/faastransform:0.0.5 }
	exec { docker push simpleidserver/faaskubernetes:0.0.5 }
	exec { docker push simpleidserver/faasgateway:0.0.5 }
	exec { docker push simpleidserver/faaswebsite:0.0.5 }
	exec { docker push simpleidserver/faasprometheus:0.0.5 }
	exec { docker push simpleidserver/eventmeshserver:0.0.5 }
}

# Kubernetes
task deployEventMeshServerInMemoryBroker {
	exec { kubectl apply -f ./kubernetes/eventmeshserver.yml --namespace=faas }	
}

task deployEventMeshServerRabbitMQ {
	exec { kubectl apply -f ./kubernetes/eventmeshserver.rabbitmq.yml --namespace=faas }	
}

task deployEventMeshServerKafka {
	exec { kubectl apply -f ./kubernetes/eventmeshserver.kafka.yml --namespace=faas }	
}

task deployServerlessWorkflow {
	exec { kubectl apply -f ./kubernetes/serverlessworkflow.yml --namespace=faas }	
}

# Publish
task publishWebsite {
	exec { git checkout gh-pages }
	exec { git rm -r . }
	exec { git checkout HEAD -- .gitignore }
	exec { git add . }
	exec { git commit -m "Remove" }
	exec { git checkout master }
	exec { docfx ./docs/docfx.json }
	exec { Copy-item -Force -Recurse -Verbose "./docs/_site/*" -Destination "." }
	exec { git checkout gh-pages --merge }
	exec { git add . }
	exec { git commit -m "Update Documentation" }
	exec { git rebase -i HEAD~2 }
	exec { git push origin gh-pages }
	exec { git checkout master }
}

task test {	
    Push-Location -Path $base_dir\tests\EventMesh.Runtime.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
	
    Push-Location -Path $base_dir\tests\FaasNet.Runtime.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
}