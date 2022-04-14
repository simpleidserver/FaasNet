'use strict';

require('source-map-support/register');
var generatorReactSdk = require('@asyncapi/generator-react-sdk');
var modelina = require('@asyncapi/modelina');
var jsxRuntime = require('react/cjs/react-jsx-runtime.production.min');

async function modelRenderer({
  asyncapi
}) {
  const generator = new modelina.CSharpGenerator({
    presets: [modelina.CSHARP_JSON_SERIALIZER_PRESET]
  });
  const generatedModels = await generator.generateCompleteModels(asyncapi, {
    namespace: 'AsyncapiEventMeshClient.Models'
  });
  const files = [];

  for (const generatedModel of generatedModels) {
    const className = modelina.FormatHelpers.toPascalCase(generatedModel.modelName);
    const modelFileName = `${className}.cs`;
    files.push( /*#__PURE__*/jsxRuntime.jsx(generatorReactSdk.File, {
      name: modelFileName,
      children: generatedModel.result
    }));
  }

  return files;
}

module.exports = modelRenderer;
//# sourceMappingURL=model.js.map
