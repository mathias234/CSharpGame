﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.Fonts {
    public class GUIText : GameComponent {
        private string _textString;
        private float _fontSize;

        private int _textMeshVao;
        private int _vertexCount;
        private Vector3 _color = new Vector3(0f, 0f, 0f);

        private Vector2 _position;
        private float _lineMaxSize;
        private int _numberOfLines;

        private FontType _font;

        private bool _centerText;

        public GUIText(string text, float fontSize, FontType font, Vector2 position, float maxLineLength,
                bool centered) {
            _textString = text;
            _fontSize = fontSize;
            _font = font;
            _position = position;
            _lineMaxSize = maxLineLength;
            _centerText = centered;
            TextMaster.LoadText(this);
        }

        public override void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine renderingEngine, string renderStage) {
            var mat = new Material(Shader.GetShader("font"));
            mat.SetMainTexture(_font.TextureAtlas);
            mat.SetVector3("color", Color);
            mat.SetVector2("translation", Position);
            mat.Shader.Bind("default");
            mat.Shader.UpdateUniforms(new Transform(), mat, CoreEngine.GetCoreEngine.RenderingEngine, "default");
            RenderText(this, mat);
        }

        public void RenderText(GUIText text, Material mat) {
            GL.BindVertexArray(text.Mesh);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, text.VertexCount);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public override void AddToEngine(ICoreEngine engine) {
            engine.GUIRenderingEngine.AddToEngine(this);
        }

        public void Remove() {
            TextMaster.RemoveText(this);
        }

        public FontType Font => _font;

        public Vector3 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public int NumberOfLines
        {
            get { return _numberOfLines; }
            set { _numberOfLines = value; }
        }

        public Vector2 Position => _position;

        public int Mesh => _textMeshVao;

        public void SetMeshInfo(int vao, int verticesCount) {
            _textMeshVao = vao;
            _vertexCount = verticesCount;
        }

        public int VertexCount => _vertexCount;

        public float FontSize => _fontSize;

        public bool IsCentered => _centerText;

        public float MaxLineSize => _lineMaxSize;

        public string TextString => _textString;
    }
}
