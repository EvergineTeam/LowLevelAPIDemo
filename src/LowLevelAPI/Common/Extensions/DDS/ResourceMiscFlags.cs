// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

/*
Copyright (c) 2012 Daniil Rodin

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

   1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

   2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

   3. This notice may not be removed or altered from any source
   distribution.
*/

using System;

namespace Evergine.Assets.Extensions.DDS
{
    [Flags]
    internal enum ResourceMiscFlags : uint
    {
        /// <summary>
        /// Specifies no flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enables an application to call ID3D10Device::GenerateMips on a texture resource.
        /// The resource must be created with the bind flags that specify that
        /// the resource is a render target and a shader resource.
        /// </summary>
        GenerateMips = 0x1,

        /// <summary>
        /// <para>Enables the sharing of resource data between two or more Direct3D devices.
        /// The only resources that can be shared are 2D non-mipmapped textures.</para>
        /// <para>WARP and REF devices do not support shared resources.
        /// Attempting to create a resource with this flag on either a WARP or REF device
        /// will cause the create method to return an E_OUTOFMEMORY error code.</para>
        /// </summary>
        Shared = 0x2,

        /// <summary>
        /// Enables an application to create a cube texture from a Texture2DArray that contains 6 textures.
        /// </summary>
        TextureCube = 0x4,

        /// <summary>
        /// <para>Enables the resource created to be synchronized using the IDXGIKeyedMutex::AcquireSync and ReleaseSync APIs.
        /// The following resource creation D3D10 APIs, that all take a D3D10_RESOURCE_MISC_FLAG parameter, have been extended to support the new flag.</para>
        /// <list type="">
        ///     ID3D10Device1::CreateTexture1D
        ///     ID3D10Device1::CreateTexture2D
        ///     ID3D10Device1::CreateTexture3D
        ///     ID3D10Device1::CreateBuffer
        /// </list>
        /// <para>If any of the listed functions are called with the D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX flag set,
        /// the interface returned can be queried for an IDXGIKeyedMutex interface,
        /// which implements AcquireSync and ReleaseSync APIs to synchronize access to the surface.
        /// The device creating the surface, and any other device opening the surface (using OpenSharedResource)
        /// is required to call IDXGIKeyedMutex::AcquireSync before any rendering commands to the surface,
        /// and IDXGIKeyedMutex::ReleaseSync when it is done rendering.</para>
        /// <para>WARP and REF devices do not support shared resources.
        /// Attempting to create a resource with this flag on either a WARP or REF device
        /// will cause the create method to return an E_OUTOFMEMORY error code.</para>
        /// </summary>
        SharedKeyedMutex = 0x10,

        /// <summary>
        /// Enables a surface to be used for GDI interoperability.
        /// Setting this flag enables rendering on the surface via IDXGISurface1::GetDC.
        /// </summary>
        GdiCompatible = 0x20,
    }
}
