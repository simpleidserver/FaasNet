# Before you start

Make sure you have [Kubernetes](https://kubernetes.io/docs/tasks/tools/) and [Helm](https://helm.sh/docs/intro/install/) installed.

Download and install the FaasNet CLI tool :

1. Download your [desired version](https://github.com/simpleidserver/FaasNet/releases).
2. Unpack it.
3. Add the Directory path into the PATH environment variable.

# Start FaasNet

Open a command prompt and create a `faas` namespace in kubernetes. 

```
kubectl create namespace faas
```

Install and launch the `faasnet` project.

```
helm repo add faasnet https://simpleidserver.github.io/FaasNet/charts/
helm install my-faasnet faasnet/faasnet --version 0.0.4 --namespace=faas
```

If the `faasnet` repository is already installed, its latest version can be downloaded by executing the following command.

```
helm repo update
```