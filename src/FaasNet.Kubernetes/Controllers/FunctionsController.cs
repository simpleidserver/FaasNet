using FaasNet.Kubernetes.Commands;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Unpublish(string id, CancellationToken cancellationToken)
        {
            using (var client = new k8s.Kubernetes(_configuration))
            {
                await DeleteNamespace(id, client, cancellationToken);
            }

            return new NoContentResult();
        }

        [HttpPost("invoke")]
        public async Task<IActionResult> Invoke([FromBody] InvokeFunctionCommand cmd, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var content = cmd.Content == null ? string.Empty : cmd.Content.ToString();
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(BuildFunctionUrl(cmd.Id)),
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
        public async Task<IActionResult> GetConfiguration(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResult = await httpClient.GetAsync($"{BuildFunctionUrl(id)}/configuration");
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
                    Name = cmd.Id
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
                    Name = $"{cmd.Id}-runtime"
                },
                Spec = new k8s.Models.V1DeploymentSpec
                {
                    Replicas = 2,
                    Selector = new k8s.Models.V1LabelSelector
                    {
                        MatchLabels = new Dictionary<string, string>
                        {
                            { cmd.Id, "web" }
                        }
                    },
                    Template = new k8s.Models.V1PodTemplateSpec
                    {
                        Metadata = new k8s.Models.V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                { cmd.Id, "web" }
                            }
                        },
                        Spec = new k8s.Models.V1PodSpec
                        {
                            Containers = new List<k8s.Models.V1Container>
                            {
                                new k8s.Models.V1Container
                                {
                                    Name = cmd.Id,
                                    Image = cmd.Image,
                                    ImagePullPolicy = "IfNotPresent"
                                }
                            }
                        }
                    }
                }
            }, cmd.Id, cancellationToken: cancellationToken);
        }

        private Task CreateService(PublishFunctionCommand cmd, k8s.Kubernetes client, CancellationToken cancellationToken)
        {
            return client.CreateNamespacedServiceAsync(new k8s.Models.V1Service
            {
                ApiVersion = "v1",
                Kind = "Service",
                Metadata = new k8s.Models.V1ObjectMeta
                {
                    Name = $"{cmd.Id}-entry",
                    NamespaceProperty = cmd.Id
                },
                Spec = new k8s.Models.V1ServiceSpec
                {
                    Type = "ClusterIP",
                    Selector = new Dictionary<string, string>
                    {
                        { cmd.Id, "web" }
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
            }, cmd.Id, cancellationToken: cancellationToken);
        }

        private static string BuildFunctionUrl(string id)
        {
            return $"http://{id}-entry.{id}.svc.cluster.local";
        }

        #endregion
    }
}
