using System.Collections.Generic;

namespace DodoBot.Models;

/// <summary>
///
/// </summary>
public class CandidateSpecialty
{
    /// <summary>
    ///
    /// </summary>
    public HashSet<int> Specialty { get; set; }

    /// <summary>
    ///
    /// </summary>
    public HashSet<int> SubSpecialty { get; set; }
}