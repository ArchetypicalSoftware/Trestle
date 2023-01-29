using k8s.Models;
using k8s;

namespace Archetypical.Software.Trestle
{
    public static class Extensions
    {
        public static Task<PortForwardedHttpClient> PortForward(this V1Pod pod, IKubernetes client, int port)
        {
            return PortForwardedHttpClient.Create(client, pod, port);
        }

        public static Task<PortForwardedHttpClient> PortForward(this IKubernetes client, V1Pod pod)
        {
            return PortForwardedHttpClient.Create(client, pod, pod.Spec.Containers.SelectMany(c => c.Ports.Select(p => p.ContainerPort)).ToArray());
        }
    }
}