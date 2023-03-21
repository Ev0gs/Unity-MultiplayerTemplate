using Unity.Netcode.Components;

public class ClientNetworkTransform : NetworkTransform
{
    /// <summary>
    /// This script override Server Authoritative mode to make transform 
    /// from the client side possible rather than using RPCs.
    /// Avoid if competitive game !
    /// </summary>
    /// <returns></returns>
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
