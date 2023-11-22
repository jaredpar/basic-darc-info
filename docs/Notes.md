Container Publishing
===
Mostly based my approach off of [this tutorial][1]. The biggest difference is that I'm using a service principal to authenticate with ACR instead of the admin role. The SP has a more limited role so this should be a safer approach.

[1]: https://learn.microsoft.com/en-us/azure/app-service/deploy-container-github-action?tabs=publish-profile