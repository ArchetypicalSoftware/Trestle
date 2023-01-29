using System.Net;
using System.Net.Sockets;
using System.Text;
using k8s;
using k8s.Models;

namespace Archetypical.Software.Trestle;

public class PortForwardedHttpClient : HttpClient, IDisposable
{
    protected readonly HttpClient Inner;
    private static Socket? _handler;
    private static Socket? _listener;

    internal PortForwardedHttpClient(HttpClient inner)
    {
        Inner = inner;
    }

    public static async Task<PortForwardedHttpClient> Create(IKubernetes client, V1Pod pod, params int[] ports)
    {
        // Note this is single-threaded, it won't handle concurrent requests well...
        var webSocket = await client.WebSocketNamespacedPodPortForwardAsync(pod.Name(), pod.Namespace(), ports, "v4.channel.k8s.io");
        var demux = new StreamDemuxer(webSocket, StreamType.PortForward);
        demux.Start();

        var stream = demux.GetStream((byte?)0, (byte?)0);

        IPAddress ipAddress = IPAddress.Loopback;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, GetRandomUnusedPort());
        _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(localEndPoint);
        _listener.Listen(100);

        // Note this will only accept a single connection
        var accept = Task.Run(() =>
        {
            while (true)
            {
                _handler = _listener.Accept();
                var bytes = new byte[4096];
                while (true)
                {
                    int bytesRec = _handler.Receive(bytes);
                    stream.Write(bytes, 0, bytesRec);
                    if (bytesRec == 0 || Encoding.ASCII.GetString(bytes, 0, bytesRec).IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
            }
        });

        var _ = Task.Run(() =>
        {
            var buff = new byte[4096];
            while (true)
            {
                var read = stream.Read(buff, 0, 4096);
                _handler.Send(buff, read, 0);
            }
        });

        var webproxy = new WebProxy(localEndPoint.ToString());
        var ht = new HttpClient(new HttpClientHandler()
        {
            Proxy = webproxy
        });

        return new PortForwardedHttpClient(ht);
    }

    public new void CancelPendingRequests()
    { Inner.CancelPendingRequests(); }

    public new Task<HttpResponseMessage> DeleteAsync(string? requestUri)
    { return Inner.DeleteAsync(requestUri); }

    public new Task<HttpResponseMessage> DeleteAsync(Uri? requestUri)
    { return Inner.DeleteAsync(requestUri); }

    public new Task<HttpResponseMessage> DeleteAsync(string? requestUri, CancellationToken cancellationToken)
    { return Inner.DeleteAsync(requestUri, cancellationToken); }

    public new Task<HttpResponseMessage> DeleteAsync(Uri? requestUri, CancellationToken cancellationToken)
    { return Inner.DeleteAsync(requestUri, cancellationToken); }

    public new void Dispose()
    {
        _handler?.Close();
        _listener?.Close();
    }

    public new Task<HttpResponseMessage> GetAsync(string? requestUri)
    { return Inner.GetAsync(requestUri); }

    public new Task<HttpResponseMessage> GetAsync(Uri? requestUri)
    { return Inner.GetAsync(requestUri); }

    public new Task<HttpResponseMessage> GetAsync(string? requestUri, HttpCompletionOption completionOption)
    { return Inner.GetAsync(requestUri, completionOption); }

    public new Task<HttpResponseMessage> GetAsync(Uri? requestUri, HttpCompletionOption completionOption)
    { return Inner.GetAsync(requestUri, completionOption); }

    public new Task<HttpResponseMessage> GetAsync(string? requestUri, CancellationToken cancellationToken)
    { return Inner.GetAsync(requestUri, cancellationToken); }

    public new Task<HttpResponseMessage> GetAsync(Uri? requestUri, CancellationToken cancellationToken)
    { return Inner.GetAsync(requestUri, cancellationToken); }

    public new Task<HttpResponseMessage> GetAsync(string? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    { return Inner.GetAsync(requestUri, cancellationToken); }

    public new Task<HttpResponseMessage> GetAsync(Uri? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    { return Inner.GetAsync(requestUri, completionOption, cancellationToken); }

    public new Task<byte[]> GetByteArrayAsync(string? requestUri)
    { return Inner.GetByteArrayAsync(requestUri); }

    public new Task<byte[]> GetByteArrayAsync(Uri? requestUri)
    { return Inner.GetByteArrayAsync(requestUri); }

    public new Task<byte[]> GetByteArrayAsync(string? requestUri, CancellationToken cancellationToken)
    { return Inner.GetByteArrayAsync(requestUri, cancellationToken); }

    public new Task<byte[]> GetByteArrayAsync(Uri? requestUri, CancellationToken cancellationToken)
    { return Inner.GetByteArrayAsync(requestUri); }

    public new Task<Stream> GetStreamAsync(string? requestUri)
    { return Inner.GetStreamAsync(requestUri); }

    public new Task<Stream> GetStreamAsync(string? requestUri, CancellationToken cancellationToken)
    { return Inner.GetStreamAsync(requestUri, cancellationToken); }

    public new Task<Stream> GetStreamAsync(Uri? requestUri)
    { return Inner.GetStreamAsync(requestUri); }

    public new Task<Stream> GetStreamAsync(Uri? requestUri, CancellationToken cancellationToken)
    { return Inner.GetStreamAsync(requestUri, cancellationToken); }

    public new Task<string> GetStringAsync(string? requestUri)
    {
        return Inner.GetStringAsync(requestUri);
    }

    public new Task<string> GetStringAsync(Uri? requestUri)
    { return Inner.GetStringAsync(requestUri); }

    public new Task<string> GetStringAsync(string? requestUri, CancellationToken cancellationToken)
    { return Inner.GetStringAsync(requestUri, cancellationToken); }

    public new Task<string> GetStringAsync(Uri? requestUri, CancellationToken cancellationToken)
    { return Inner.GetStringAsync(requestUri, cancellationToken); }

    public new Task<HttpResponseMessage> PatchAsync(string? requestUri, HttpContent? content)
    { return Inner.PatchAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PatchAsync(Uri? requestUri, HttpContent? content)
    { return Inner.PatchAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PatchAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PatchAsync(requestUri, content, cancellationToken); }

    public new Task<HttpResponseMessage> PatchAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PatchAsync(requestUri, content, cancellationToken); }

    public new Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content)
    { return Inner.PostAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PostAsync(Uri? requestUri, HttpContent? content)
    { return Inner.PostAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PostAsync(requestUri, content, cancellationToken); }

    public new Task<HttpResponseMessage> PostAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PostAsync(requestUri, content, cancellationToken); }

    public new Task<HttpResponseMessage> PutAsync(string? requestUri, HttpContent? content)
    { return Inner.PutAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PutAsync(Uri? requestUri, HttpContent? content)
    { return Inner.PutAsync(requestUri, content); }

    public new Task<HttpResponseMessage> PutAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PutAsync(requestUri, content, cancellationToken); }

    public new Task<HttpResponseMessage> PutAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
    { return Inner.PutAsync(requestUri, content, cancellationToken); }

    public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    { return Inner.Send(request, cancellationToken); }

    public new HttpResponseMessage Send(HttpRequestMessage request)
    { return Inner.Send(request); }

    public new HttpResponseMessage Send(HttpRequestMessage request, HttpCompletionOption completionOption)
    { return Inner.Send(request, completionOption); }

    public new HttpResponseMessage Send(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    { return Inner.Send(request, cancellationToken); }

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    { return Inner.SendAsync(request, cancellationToken); }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    { return Inner.SendAsync(request); }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
    { return Inner.SendAsync(request, completionOption); }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    { return Inner.SendAsync(request, completionOption, cancellationToken); }

    internal static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}