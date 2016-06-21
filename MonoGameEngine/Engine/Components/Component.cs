﻿using System;
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Components {
    [XmlInclude(typeof(Camera))]
    [XmlInclude(typeof(Transform))]
    [XmlInclude(typeof(MeshRenderer))]
    [XmlInclude(typeof(BoxCollider))]
    [XmlInclude(typeof(SphereCollider))]
    [XmlInclude(typeof(UIComponent))]
    [XmlInclude(typeof(UiTextComponent))]
    [XmlInclude(typeof(UiTextureComponent))]
    [XmlInclude(typeof(UIComponent))]
    public class Component  {
        [XmlIgnore]
        public GameObject GameObject;

        [XmlIgnore]
        public Transform Transform => GameObject.Transform;

        public virtual void Update(float deltaTime) { }
        public virtual void Draw(GraphicsDevice graphicsDevice) { }

        public virtual void Init() { }
    }
}
