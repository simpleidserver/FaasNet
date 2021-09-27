using FaasNet.Kubernetes.Commands;
using FaasNet.Kubernetes.Results;
using k8s;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Kubernetes.Controllers
{
    [Route("functions")]
    public class FunctionsController : Controller
    {
        private readonly KubernetesClientConfiguration _configuration;

        public FunctionsController(KubernetesClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Operations

        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] PublishFunctionCommand cmd, CancellationToken cancellationToken)
        {
            using (var client = new k8s.Kubernetes(_configuration))
            {
                await CreateNamespace(cmd, client, cancellationToken);
                await CreateDeployment(cmd, client, cancellationToken);
                await CreateService(cmd, client, cancellationToken);
            }

            return new NoContentResult();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Unpublish(string name, CancellationToken cancellationToken)
        {
            using (var client = new k8s.Kubernetes(_configuration))
            {
                await DeleteNamespace(name, client, cancellationToken);
            }

            return new NoContentResult();
        }

        [HttpGet("{name}/details")]
        public async Task<IActionResult> GetDetails(string name)
        {
            using (var client = new k8s.Kubernetes(_configuration))
            {
                var pods = await client.ListNamespacedPodAsync(name);
                return new OkObjectResult(FunctionMonitoringResult.Build(pods));
            }
        }

        [HttpPost("invoke")]
        public async Task<IActionResult> Invoke([FromBody] InvokeFunctionCommand cmd, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var content = cmd.Content == null ? string.Empty : cmd.Content.ToString();
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(BuildFunctionUrl(cmd.Name)),
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(request, cancellationToken);
                var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                return new ContentResult
                {
                    StatusCode = (int)httpResult.StatusCode,
                    Content = json,
                    ContentType = "application/json"
                };
            }
        }

        [HttpGet("{name}/configuration")]
        public async Task<IActionResult> GetConfiguration(string name)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResult = await httpClient.GetAsync($"{BuildFunctionUrl(name)}/configuration");
                var json = await httpResult.Content.ReadAsStringAsync();
                return new ContentResult
                {
                    StatusCode = (int)httpResult.StatusCode,
                    Content = json,
                    ContentType = "application/json"
                };
            }
        }

        #endregion

        #region Private methods

        private Task CreateNamespace(PublishFunctionCommand cmd, k8s.Kubernetes kubernetes, CancellationToken cancellationToken)
        {
            return kubernetes.CreateNamespaceAsync(new k8s.Models.V1Namespace
            {
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = cmd.Name
                }
            }, cancellationToken: cancellationToken);
        }

        private Task DeleteNamespace(string name, k8s.Kubernetes kubernetes, CancellationToken cancellationToken)
        {
            return kubernetes.DeleteNamespaceAsync(name, cancellationToken: cancellationToken);
        }

        private Task CreateDeployment(PublishFunctionCommand cmd, k8s.Kubernetes client, CancellationToken cancellationToken)
        {
            return client.CreateNamespacedDeploymentAsync(new k8s.Models.V1Deployment
            {
                Kind = "Deployment",
                ApiVersion = "apps/v1",
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = $"{cmd.Name}-runtime"
                },
                Spec = new k8s.Models.V1DeploymentSpec
                {
                    Replicas = 2,
                    Selector = new k8s.Models.V1LabelSelector
                    {
                        MatchLabels = new Dictionary<string, string>
                        {
                            { cmd.Name, "web" }
                        }
                    },
                    Template = new k8s.Models.V1PodTemplateSpec
                    {
                        Metadata = new k8s.Models.V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                { cmd.Name, "web" }
                            }
                        },
                        Spec = new k8s.Models.V1PodSpec
                        {
                            Containers = new List<k8s.Models.V1Container>
                            {
                                new k8s.Models.V1Container
                                {
                                    Name = cmd.Name,
                                    Image = cmd.Image,
                                    ImagePullPolicy = "IfNotPresent"
                                }
                            }
                        }
                    }
                }
            }, cmd.Name, cancellationToken: cancellationToken);
        }

        private Task CreateService(PublishFunctionCommand cmd, k8s.Kubernetes client, CancellationToken cancellationToken)
        {
            return client.CreateNamespacedServiceAsync(new k8s.Models.V1Service
            {
                ApiVersion = "v1",
                Kind = "Service",
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = $"{cmd.Name}-entry",
                    NamespaceProperty = cmd.Name
                },
                Spec = new k8s.Models.V1ServiceSpec
                {
                    Type = "ClusterIP",
                    Selector = new Dictionary<string, string>
                    {
                        { cmd.Name, "web" }
                    },
                    Ports = new List<k8s.Models.V1ServicePort>
                    {
                        new k8s.Models.V1ServicePort
                        {
                            Port = 80,
                            TargetPort = 8080
                        }
                    }
                }
            }, cmd.Name, cancellationToken: cancellationToken);
        }

        private static string BuildFunctionUrl(string name)
        {
            return $"http://{name}-entry.{name}.svc.cluster.local";
        }

        #endregion
    }
}
