using System.Collections;
using Unity.Netcode.Components;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// This script override Server Authoritative mode to make animation for the client instant 
    /// rather than with latency waiting for server response.
    /// </summary>
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}