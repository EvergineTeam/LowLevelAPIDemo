// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents the types of textures that can be applied to an OBJ model.
    /// </summary>
    public enum OBJTextureType
    {
        /// <summary>
        /// A 2D texture mapping.
        /// </summary>
        None,

        /// <summary>
        /// A spherical texture mapping.
        /// </summary>
        Sphere,

        /// <summary>
        /// A texture applied to the top face of a cube.
        /// </summary>
        CubeTop,

        /// <summary>
        /// A texture applied to the bottom face of a cube.
        /// </summary>
        CubeBottom,

        /// <summary>
        /// A texture applied to the front face of a cube.
        /// </summary>
        CubeFront,

        /// <summary>
        /// A texture applied to the back face of a cube.
        /// </summary>
        CubeBack,

        /// <summary>
        /// A texture applied to the left face of a cube.
        /// </summary>
        CubeLeft,

        /// <summary>
        /// A texture applied to the right face of a cube.
        /// </summary>
        CubeRight,
    }
}
