using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.Camera
{
    public class OrthographicCamera : CameraBase
    {
        protected float _width;
        public float Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    projectionDirty = true;
                }
            }
        }

        protected float _height;
        public float Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
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

        public OrthographicCamera()
        {
            Position = Vector3.Zero;
            Target = Vector3.UnitZ;
            Up = Vector3.Up;

            Width = 800;
            Height = 600;
            NearPlane = 1;
            FarPlane = 100;
        }

        protected override Matrix CalculateProjection()
        {
            return Matrix.CreateOrthographic(Width, Height, NearPlane, FarPlane);
        }
    }
}