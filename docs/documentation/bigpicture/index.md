# Introduction

The FaasNet architecture is made of 6 blocks :

* **Function** : Each function is deployed into one or more Kubernetes PODs.
* **Gateway** : Expose operations to the angular website & CLI. They are used to manage the lifecyle of the functions.
* **Monitoring** : Prometheus product is configured by default in order to monitor the published functions.
* **Website** : Portal application which can be used by developers to manage the functions.
* **CLI** : Command Line Interface (CLI) which can be used by developers to manage the functions.
* **FaasNet Kubernetes** : API used by the gateway to interact with Kubernetes.

![Big Picture](images/bigpicture1.png)