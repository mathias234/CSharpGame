﻿using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components {
    public class WaterMesh : GameComponent {
        private Mesh _waterMesh;
        private Material _material;
        private float _waveSpeed;

        public WaterMesh(int width, int height, Vector4 waterColor, float waveSpeed = 0.01f, float waveStrength = 0.02f, float refractivePower = 0.5f, float dudvTiling = 6) {
            _waveSpeed = waveSpeed;

            Vertex[] vertices = {
                new Vertex(new Vector3(0, 0, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(0, 0, height), new Vector2(1, 1)),
                new Vertex(new Vector3(width, 0, height), new Vector2(0, 1)),
                new Vertex(new Vector3(width, 0, 0), new Vector2(0, 0))
            };

            int[] indices = {
                2, 0, 1,
                3, 0, 2
            };

            _waterMesh = Mesh.GetMesh(vertices, indices, true);

            _material = new Material(Shader.GetShader("water/water"));
            _material.SetMainTexture(Texture.GetTexture("dudvMap.png"));
            _material.SetFloat("specularIntensity", 3);
            _material.SetFloat("specularPower", 32);
            _material.SetTexture("normalMap", Texture.GetTexture("matchingNormalMap.png"));

            _material.SetTexture("reflectionTexture", Texture.GetTexture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear));
            _material.SetTexture("refractionTexture", Texture.GetTexture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear));
            _material.SetTexture("refractionTextureDepth", Texture.GetTexture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent, false, FramebufferAttachment.DepthAttachment));

            _material.SetFloat("moveFactor", 0);
            _material.SetFloat("waveStrength", waveStrength);
            _material.SetFloat("refractivePower", refractivePower);
            _material.SetFloat("tiling", dudvTiling);

            _material.SetVector4("waterColor", waterColor);
        }

        public override void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine baseRenderingEngine, string renderStage) {
            RenderingEngine renderingEngine;
            if (baseRenderingEngine is RenderingEngine) {
                renderingEngine = (RenderingEngine)baseRenderingEngine;
            }
            else {
                LogManager.Error("called in wrong engine");
                return;
            }

            if (renderStage.ToLower() == "refract" || renderStage.ToLower() == "reflect") {
                return;
            }

            if (!_material.Shader.GetShaderTypes.Contains(shaderType))
                return;

            var distance = 2 * (renderingEngine.MainCamera.Transform.Position.Y - Transform.Position.Y);

            renderingEngine.MainCamera.Transform.Position -= new Vector3(0, distance, 0);
            renderingEngine.MainCamera.Transform.Rotation = renderingEngine.MainCamera.Transform.Rotation.InvertPitch();

            renderingEngine.SetVector4("clipPlane", new Vector4(0, 1, 0, -(Transform.Position.Y + 0.1f)));
            renderingEngine.RenderObject(_material.GetTexture("reflectionTexture"), deltaTime, "reflect", false);

            renderingEngine.MainCamera.Transform.Position += new Vector3(0, distance, 0);
            renderingEngine.MainCamera.Transform.Rotation = renderingEngine.MainCamera.Transform.Rotation.InvertPitch(); ;

            renderingEngine.SetVector4("clipPlane", new Vector4(0, -1, 0, Transform.Position.Y + 0.5f));
            renderingEngine.RenderObject(_material.GetTexture("refractionTexture"), deltaTime, "refract", false);
            renderingEngine.RenderObject(_material.GetTexture("refractionTextureDepth"), deltaTime, "refract", false);

            renderingEngine.SetVector4("clipPlane", new Vector4(0, 0, 0, 0));

            _material.SetFloat("moveFactor", _material.GetFloat("moveFactor") + _waveSpeed * deltaTime);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _material.Shader.Bind(shaderType);
            _material.Shader.UpdateUniforms(gameObject.Transform, _material, renderingEngine, shaderType);
            _waterMesh.Draw();

            GL.Disable(EnableCap.Blend);

        }

        public override void AddToEngine(ICoreEngine engine) {
            engine.RenderingEngine.AddToEngine(this);
        }
    }
}