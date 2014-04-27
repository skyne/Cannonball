using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.Camera
{
    public class PerspectiveCamera : CameraBase
    {
        protected float _fov;
        public float FieldOfView
        {
            get { return _fov; }
            set
            {
                if (_fov != value)
                {
                    _fov = value;
                    projectionDirty = true;
                }
            }
        }

        protected float _aspectRatio;
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (_aspectRatio != value)
                {
                    _aspectRatio = value;
                    projectionDirty = true;
                }
            }
        }

        protected float _nearPlane;
        public float NearPlane
        {
            get { return _nearPlane; }
            set
            {
                if (_nearPlane != value)
                {
                    _nearPlane = value;
                    projectionDirty = true;
                }
            }
        }

        protected float _farPlane;
        public float FarPlane
        {
            get { return _farPlane; }
            set
            {
                if (_farPlane != value)
                {
                    _farPlane = value;
                    projectionDirty = true;
                }
            }
        }

        public PerspectiveCamera()
        {
            Position = Vector3.Zero;
            Target = Vector3.UnitZ;
            Up = Vector3.Up;

            FieldOfView = MathHelper.ToRadians(45.0f);
            AspectRatio = 4f / 3f;
            NearPlane = 1;
            FarPlane = 100;
        }

        protected override Matrix CalculateProjection()
        {
            return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }
    }
}