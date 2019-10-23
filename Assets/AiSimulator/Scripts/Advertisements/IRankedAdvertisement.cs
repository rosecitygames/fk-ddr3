using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An extended interface for advertisements that includes a rank value.
    /// </summary>
    public interface IRankedAdvertisement : IAdvertisement
    {
        /// <summary>
        /// An arbitrary rank that can be assigned to the advertisement.
        /// </summary>
        int Rank { get; set; }
    }
}
