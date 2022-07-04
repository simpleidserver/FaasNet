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

# CI tasks
task ci -depends compile, publishDockerEventMeshService, publishEventMeshCLI, pack

task clean {
	rd "$source_dir\artifacts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	rd "$base_dir\build" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task compile -depends clean {
	echo "build: Tag is $tag"
	echo "build: Package version suffix is $suffix"
	echo "build: Build version suffix is $buildSuffix" 
	
	exec { dotnet --version }
	exec { dotnet --info }

	exec { msbuild -version }
	
    exec { dotnet build .\FaasNet.RaftConsensus.sln -c $config --version-suffix=$buildSuffix }
    exec { dotnet build .\FaasNet.EventMesh.sln -c $config --version-suffix=$buildSuffix }
    exec { dotnet build .\FaasNet.DHT.sln -c $config --version-suffix=$buildSuffix }
}

# Publish EventMesh
task publishDockerEventMeshService {
	rd "$result_dir\eventMeshService" -recurse -force  -ErrorAction SilentlyContinue | out-null
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Service\FaasNet.EventMesh.Service.csproj -c $config -o $result_dir\eventMeshService }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Protocols.AMQP\FaasNet.EventMesh.Protocols.AMQP.csproj -c $config -o $result_dir\protocolPlugins\FaasNet.EventMesh.Protocols.AMQP }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Protocols.WebSocket\FaasNet.EventMesh.Protocols.WebSocket.csproj -c $config -o $result_dir\protocolPlugins\FaasNet.EventMesh.Protocols.WebSocket }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.AMQP\FaasNet.EventMesh.Sink.AMQP.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.AMQP }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.Kafka\FaasNet.EventMesh.Sink.Kafka.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.Kafka }
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMesh.Sink.VpnBridge\FaasNet.EventMesh.Sink.VpnBridge.csproj -c $config -o $result_dir\sinkPlugins\FaasNet.EventMesh.Sink.VpnBridge }
	exec { dotnet publish $source_dir\RaftConsensus\FaasNet.RaftConsensus.Discovery.Config\FaasNet.RaftConsensus.Discovery.Config.csproj -c $config -o $result_dir\discoveryPlugins\FaasNet.RaftConsensus.Discovery.Config }
	exec { dotnet publish $source_dir\RaftConsensus\FaasNet.RaftConsensus.Discovery.Etcd\FaasNet.RaftConsensus.Discovery.Etcd.csproj -c $config -o $result_dir\discoveryPlugins\FaasNet.RaftConsensus.Discovery.Etcd }
}

task publishEventMeshCLI {
	exec { dotnet publish $source_dir\EventMesh\FaasNet.EventMeshCTL.CLI\FaasNet.EventMeshCTL.CLI.csproj -c $config -o $result_dir\eventMeshCLI }
}

# Publish Nuget package 
task pack {
	exec { dotnet pack $source_dir\Common\FaasNet.Common\FaasNet.Common.csproj -c $config --no-build $versionSuffix --output $result_dir\nugetPackages }
	exec { dotnet pack $source_dir\DHT\FaasNet.DHT.Chord.Client\FaasNet.DHT.Chord.Client.csproj -c $config --no-build $versionSuffix --output $result_dir\nugetPackages }
	exec { dotnet pack $source_dir\DHT\FaasNet.DHT.Chord.Core\FaasNet.DHT.Chord.Core.csproj -c $config --no-build $versionSuffix --output $result_dir\nugetPackages }
	exec { dotnet pack $source_dir\DHT\FaasNet.DHT.Kademlia.Client\FaasNet.DHT.Kademlia.Client.csproj -c $config --no-build $versionSuffix --output $result_dir\nugetPackages }
	exec { dotnet pack $source_dir\DHT\FaasNet.DHT.Kademlia.Core\FaasNet.DHT.Kademlia.Core.csproj -c $config --no-build $versionSuffix --output $result_dir\nugetPackages }
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

# Test
task test {	
    Push-Location -Path $base_dir\tests\FaasNet.EventMesh.Runtime.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
	
    Push-Location -Path $base_dir\tests\FaasNet.RaftConsensus.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
}

# Publish Docker
task publishDockerEventMesh {
	[xml]$XmlDoc = Get-Content -Path .\Directory.Build.props
	$VersionPrefix = $XmlDoc.Project.PropertyGroup.VersionPrefix
	docker build -t simpleidserver/faaseventmesh:$VersionPrefix -f EventMeshDockerFile .
	docker push simpleidserver/faaseventmesh:$VersionPrefix
}