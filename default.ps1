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
    exec { dotnet build .\FaasNet.EventMesh.sln -c $config --version-suffix=$buildSuffix }
}

# Publish assets
task publishCLI {
	# exec { dotnet publish $source_dir\Clients\FaasNet.CLI\FaasNet.CLI.csproj -c $config -o $result_dir\cli }
}

task publishDocker {
	exec { dotnet publish $source_dir\Function\FaasNet.Function.GetSql\FaasNet.Function.GetSql.csproj -c $config -o $result_dir\services\RuntimeGetSql }
	exec { dotnet publish $source_dir\Function\FaasNet.Function.Transform\FaasNet.Function.Transform.csproj -c $config -o $result_dir\services\RuntimeTransform }
	exec { dotnet publish $source_dir\Function\FaasNet.Kubernetes\FaasNet.Kubernetes.csproj -c $config -o $result_dir\services\Kubernetes }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.SqlServer.Startup\FaasNet.EventMesh.SqlServer.Startup.csproj -c $config -o $result_dir\services\EventMeshApi }
	exec { dotnet publish $source_dir\Application\FaasNet.Application.SqlServer.Startup\FaasNet.Application.SqlServer.Startup.csproj -c $config -o $result_dir\services\Application }
	exec { dotnet publish $source_dir\Function\FaasNet.Function.SqlServer.Startup\FaasNet.Function.SqlServer.Startup.csproj -c $config -o $result_dir\services\FunctionApi }
	exec { dotnet publish $source_dir\StateMachines\FaasNet.StateMachine.SqlServer.Startup\FaasNet.StateMachine.SqlServer.Startup.csproj -c $config -o $result_dir\services\StateMachineApi }
	exec { dotnet publish $source_dir\Gateway\FaasNet.Gateway.Startup\FaasNet.Gateway.Startup.csproj -c $config -o $result_dir\services\Gateway }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Runtime.Website\FaasNet.EventMesh.Runtime.Website.csproj -c $config -o $result_dir\services\EventMeshServer }
}

task publishEventMeshPlugins {
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Protocols.AMQP\FaasNet.EventMesh.Protocols.AMQP.csproj -c $config -o $result_dir\protocolPlugins\FaasNet.EventMesh.Protocols.AMQP }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Protocols.WebSocket\FaasNet.EventMesh.Protocols.WebSocket.csproj -c $config -o $result_dir\protocolPlugins\FaasNet.EventMesh.Protocols.WebSocket }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.AMQP\FaasNet.EventMesh.Sink.AMQP.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.AMQP }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.Kafka\FaasNet.EventMesh.Sink.Kafka.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.Kafka }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.VpnBridge\FaasNet.EventMesh.Sink.VpnBridge.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.VpnBridgea }
}

task publishEventMeshService {
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Service\FaasNet.EventMesh.Service.csproj -c $config -o $result_dir\eventMeshService }
}

task publishEventMeshCLI {
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMeshCTL.CLI\FaasNet.EventMeshCTL.CLI.csproj -c $config -o $result_dir\eventMeshCLI }
}

task deployEventMeshService {
	exec { sc.exe create "EventMesh Service" binpath="$result_dir\eventMeshService\FaasNet.EventMesh.Service.exe --contentRoot $result_dir\eventMeshService" }
}

# Pack
task pack -depends release, compile {
	exec { dotnet pack $source_dir\Function\FaasNet.Function\FaasNet.Function.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Client\FaasNet.EventMesh.Client.csproj -c $config -no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime\FaasNet.EventMesh.Runtime.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.AMQP\FaasNet.EventMesh.Runtime.AMQP.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.EF\FaasNet.EventMesh.Runtime.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.Kafka\FaasNet.EventMesh.Runtime.Kafka.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task packNoSuffix -depends release, compile {
	exec { dotnet pack $source_dir\Function\FaasNet.Function\FaasNet.Function.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Client\FaasNet.EventMesh.Client.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime\FaasNet.EventMesh.Runtime.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.AMQP\FaasNet.EventMesh.Runtime.AMQP.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.EF\FaasNet.EventMesh.Runtime.EF.csproj -c $config --output $result_dir }
	exec { dotnet pack $source_dir\EventMesh\FaasNet.EventMesh.Runtime.Kafka\FaasNet.EventMesh.Runtime.Kafka.csproj -c $config --output $result_dir }
}

task packTemplate {
	exec { dotnet pack $source_dir\Templates\FaasNet.Templates\FaasNet.Templates.csproj -c $config --no-build --output $result_dir }
}

# Docker
task buildDockerDev -depends publishDocker {
	exec { npm run docker --prefix $source_dir\Clients\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t localhost:5000/faasgetsql . }
	exec { docker build -f RuntimeTransformDockerfile -t localhost:5000/faastransform . }
	exec { docker build -f KubernetesDockerfile -t localhost:5000/faaskubernetes . }
	exec { docker build -f EventMeshApiDockerFile -t localhost:5000/faaseventmesh . }
	exec { docker build -f ApplicationDockerFile -t localhost:5000/faasapplication . }
	exec { docker build -f FunctionDockerFile -t localhost:5000/faasfunction . }
	exec { docker build -f StateMachineDockerFile -t localhost:5000/faasstatemachine . }
	exec { docker build -f GatewayDockerfile -t localhost:5000/faasgateway . }
	exec { docker build -f WebsiteDockerfile -t localhost:5000/faaswebsite . }
	exec { docker build -f PrometheusDockerfile -t localhost:5000/faasprometheus . }
	exec { docker build -f EventMeshDockerFile -t localhost:5000/eventmeshserver . }
}

task buildDockerCI -depends publishDocker {
	exec { npm run docker --prefix $source_dir\FaasNet.Website }
	exec { docker build -f RuntimeGetSqlDockerfile -t simpleidserver/faasgetsql:0.0.6 . }
	exec { docker build -f RuntimeTransformDockerfile -t simpleidserver/faastransform:0.0.6 . }
	exec { docker build -f KubernetesDockerfile -t simpleidserver/faaskubernetes:0.0.6 . }
	exec { docker build -f EventMeshApiDockerFile -t simpleidserver/faaseventmesh:0.0.6 . }
	exec { docker build -f ApplicationDockerFile -t simpleidserver/faasapplication:0.0.6 . }
	exec { docker build -f FunctionDockerFile -t simpleidserver/faasfunction:0.0.6 . }
	exec { docker build -f StateMachineDockerFile -t simpleidserver/faasstatemachine:0.0.6 . }
	exec { docker build -f GatewayDockerfile -t simpleidserver/faasgateway:0.0.6 . }
	exec { docker build -f WebsiteDockerfile -t simpleidserver/faaswebsite:0.0.6 . }
	exec { docker build -f PrometheusDockerfile -t simpleidserver/faasprometheus:0.0.6 . }
	exec { docker build -f EventMeshDockerFile -t simpleidserver/eventmeshserver:0.0.6 . }
}

task publishDockerDev -depends buildDockerDev {
	exec { docker push localhost:5000/faasgetsql }
	exec { docker push localhost:5000/faastransform }
	exec { docker push localhost:5000/faaskubernetes }
	exec { docker push localhost:5000/faaseventmesh }
	exec { docker push localhost:5000/faasapplication }
	exec { docker push localhost:5000/faasfunction }
	exec { docker push localhost:5000/faasstatemachine }
	exec { docker push localhost:5000/faasgateway }
	exec { docker push localhost:5000/faaswebsite }
	exec { docker push localhost:5000/faasprometheus }
	exec { docker push localhost:5000/eventmeshserver }
}

task publishDockerCI -depends buildDockerCI {
	exec { docker push simpleidserver/faasgetsql:0.0.6 }
	exec { docker push simpleidserver/faastransform:0.0.6 }
	exec { docker push simpleidserver/faaskubernetes:0.0.6 }
	exec { docker push simpleidserver/faaseventmesh:0.0.6 }
	exec { docker push simpleidserver/faasapplication:0.0.6 }
	exec { docker push simpleidserver/faasfunction:0.0.6 }
	exec { docker push simpleidserver/faasstatemachine:0.0.6 }
	exec { docker push simpleidserver/faasgateway:0.0.6 }
	exec { docker push simpleidserver/faaswebsite:0.0.6 }
	exec { docker push simpleidserver/faasprometheus:0.0.6 }
	exec { docker push simpleidserver/eventmeshserver:0.0.6 }
}

# Kubernetes
task deployEvtMeshServerInMemoryBroker {
	exec { kubectl apply -f ./kubernetes/EvtMeshserver.yml --namespace=faas }	
}

task deployEvtMeshServerRabbitMQ {
	exec { kubectl apply -f ./kubernetes/EvtMeshserver.rabbitmq.yml --namespace=faas }	
}

task deployEvtMeshServerKafka {
	exec { kubectl apply -f ./kubernetes/EvtMeshserver.kafka.yml --namespace=faas }	
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
    Push-Location -Path $base_dir\tests\FaasNet.EventMesh.Runtime.Tests

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