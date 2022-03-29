import { File } from '@asyncapi/generator-react-sdk';
// eslint-disable-next-line no-unused-vars
import { AsyncAPIDocument } from '@asyncapi/parser';

/**
 * @typedef RenderArgument
 * @type {object}
 * @property {AsyncAPIDocument} asyncapi received from the generator.
 */

/**
 *
 * @param {RenderArgument} param0
 * @returns
 */
export default function asyncapiEventMeshClient({ params }) {
  return <File name={'AsyncapiEventMeshClient.csproj'}>
    {`
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <RootNamespace>AsyncapiEventMeshClient</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="FaasNet.EventMesh.Client" Version="0.0.6" />
  </ItemGroup>
</Project>`
    }
  </File>;
}