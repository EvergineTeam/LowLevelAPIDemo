// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a joint and its associated weight used in skeletal animation for OBJ models.
    /// </summary>
    public struct OBJJointAndWeight
    {
        /// <summary>
        /// Gets or sets the identifier of the joint.
        /// </summary>
        public int JointId;

        /// <summary>
        /// Gets or sets the weight value that determines the influence of the joint.
        /// Value typically ranges from 0 to 1, where 0 means no influence and 1 means full influence.
        /// </summary>
        public float Weight;
    }
}
