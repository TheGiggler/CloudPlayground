using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;
using k8s.KubeConfigModels;

namespace EdgeService.Controllers
{
    [Route("api/time")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult<string> Get()
        {
            //get HOSTNAME variable for Pod
            var hostnameMachine = Environment.GetEnvironmentVariable("HOSTNAME", EnvironmentVariableTarget.Machine);
            var hostnameProcess = Environment.GetEnvironmentVariable("HOSTNAME", EnvironmentVariableTarget.Process);
            var hostnameUser = Environment.GetEnvironmentVariable("HOSTNAME", EnvironmentVariableTarget.User);

            //get Service1 uri and port
            //Ex: 
            //Ex:,SERVICE1_SERVICE_PORT_8765_TCP_PORT = 8765
            //
            //want to "discover" Service 1 and call its Time endpoint
            //clusterUri/api/v1/namespaces/default/services : list of services
            //clusterUri/api/v1/namespaces/default/services/service1-service :details of service1-service
            //http://fcg-k8s-dns-ff55be61.hcp.eastus.azmk8s.io:443/api/v1/namespaces/default/services/service1-service
            //unfortunately, this uri doesn't seem to work ...returns nonsense
            //In the cluster, we can use just the service name and port as configured in our yakl deployment file.
            //Theoretically, you should be able to test this code locally using Azure Dev Spaces to debug.
            //I have not been successful.  Always some connection issue with the container.
            //Also tried using the KubernetesClient library but ran into SSL issues.

            System.Net.WebClient client = new System.Net.WebClient();
            //can use env variables here to get virtual ip and port for sevice1-service
            //Ex: SERVICE1_SERVICE_SERVICE_HOST=10.0.61.236
            //SERVICE1_SERVICE_PORT = SERVICE1_SERVICE_SERVICE_PORT=8765
            //SERVICE1_SERVICE_SERVICE_HOST=10.0.113.246
            var service1Port = Environment.GetEnvironmentVariable("SERVICE1_SERVICE_SERVICE_PORT", EnvironmentVariableTarget.Process);
            var service1ServiceHost= Environment.GetEnvironmentVariable("SERVICE1_SERVICE_SERVICE_HOST", EnvironmentVariableTarget.Process);

            var uri = $"http://{service1ServiceHost}:{service1Port}/api/time";
            var time = client.DownloadString(uri);
            //or use service name and DNS
            //var time = client.DownloadString("http://service1-service:8765/api/time");

            return time;

            //if (!String.IsNullOrEmpty(hostnameMachine))
            //{
            //    return $"Hostname:{hostnameMachine}; Time: {time}";  
            //}
            //else
            //{


            //    return time + $"machine {hostnameMachine}, process {hostnameProcess}, user {hostnameUser}";
            //}
            //return DateTime.UtcNow;
        }

    }
}