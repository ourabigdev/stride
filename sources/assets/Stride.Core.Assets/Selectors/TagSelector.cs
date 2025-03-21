// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Serialization.Contents;

namespace Stride.Core.Assets.Selectors;

/// <summary>
/// An <see cref="AssetSelector"/> using tags stored in <see cref="Asset.Tags"/>
/// </summary>
[DataContract("TagSelector")]
public class TagSelector : AssetSelector
{
    /// <summary>
    /// Gets the tags that will be used to select an asset.
    /// </summary>
    /// <value>The tags.</value>
    public TagCollection Tags { get; } = [];

    public override IEnumerable<string> Select(PackageSession packageSession, IContentIndexMap contentIndexMap)
    {
        return packageSession.Packages
            .SelectMany(package => package.Assets) // Select all assets
            .Where(assetItem => assetItem.Asset.Tags.Any(tag => Tags.Contains(tag))) // Matches tags
            .Select(x => x.Location.FullPath); // Convert to string;
    }
}
