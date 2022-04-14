'use strict';

require('source-map-support/register');
var generatorReactSdk = require('@asyncapi/generator-react-sdk');
require('@asyncapi/parser');
var jsxRuntime = require('react/cjs/react-jsx-runtime.production.min');

function asyncapiEventMeshClient({
  params
}) {
  return /*#__PURE__*/jsxRuntime.jsx(generatorReactSdk.File, {
    name: 'AsyncapiEventMeshClient.csproj',
    children: `
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <RootNamespace>AsyncapiEventMeshClient</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="FaasNet.EventMesh.Client" Version="0.0.6" />
  </ItemGroup>
</Project>`
  });
}

module.exports = asyncapiEventMeshClient;
//# sourceMappingURL=AsyncapiEventMeshClient.csproj.js.map
