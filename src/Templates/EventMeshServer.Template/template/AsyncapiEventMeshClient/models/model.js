import { File } from '@asyncapi/generator-react-sdk';
import { CSharpGenerator, FormatHelpers, CSHARP_JSON_SERIALIZER_PRESET} from '@asyncapi/modelina';

/**
 * @typedef RenderArgument
 * @type {object}
 * @property {AsyncAPIDocument} asyncapi received from the generator.
 */

/**
 * Render all schema models
 * @param {RenderArgument} param0
 * @returns
 */
export default async function modelRenderer({ asyncapi }) {
  const generator = new CSharpGenerator({presets: [CSHARP_JSON_SERIALIZER_PRESET]});
  const generatedModels = await generator.generateCompleteModels(asyncapi, {
    namespace: 'AsyncapiEventMeshClient.Models'
  });
  const files = [];
  for (const generatedModel of generatedModels) {
    const className = FormatHelpers.toPascalCase(generatedModel.modelName);
    const modelFileName = `${className}.cs`;
    files.push(<File name={modelFileName}>{generatedModel.result}</File>);
  }
  return files;
}