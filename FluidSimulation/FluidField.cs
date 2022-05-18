﻿using System;
using System.Collections.Generic;
using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.FluidSimulation
{
    // Details about the math operations are present in the shader files.
    // Please do not change this system too much without contacting me first. -Dominic
    public class FluidField : IDisposable
    {
        internal RenderTarget2D TemporaryAuxilaryTarget;

        internal FluidFieldState HorizontalFieldSpeed;

        internal FluidFieldState VerticalFieldSpeed;

        internal FluidFieldState DensityField;

        internal FluidFieldState ColorField;

        public float Viscosity;

        public float DiffusionFactor;

        public float DissipationFactor;

        public bool ShouldUpdate;

        public bool ShouldSkipDivergenceClearingStep;

        public Action UpdateAction;

        public bool Disposing
        {
            get;
            private set;
        }

        public readonly int Size;

        public readonly float Scale;

        public const float DeltaTime = 0.016666f;

        public const int GaussSeidelIterations = 3;

        internal static BasicEffect basicShader = null;

        public static BasicEffect BasicShader
        {
            get
            {
                if (Main.netMode != NetmodeID.Server && basicShader is null)
                {
                    basicShader = new BasicEffect(Main.instance.GraphicsDevice)
                    {
                        VertexColorEnabled = true,
                        TextureEnabled = true
                    };
                }
                return basicShader;
            }
        }

        internal FluidField(int size, float scale, float viscosity, float diffusionFactor, float dissipationFactor)
        {
            Size = size;
            Scale = scale;
            Viscosity = viscosity;
            DiffusionFactor = diffusionFactor;
            DissipationFactor = dissipationFactor;

            HorizontalFieldSpeed = new(size, SurfaceFormat.Vector4);
            VerticalFieldSpeed = new(size, SurfaceFormat.Vector4);
            DensityField = new(size);
            ColorField = new(size);

            // A surface format of Vector4 is used here to allow for both 0-1 ranged colors and other things at the same time.
            TemporaryAuxilaryTarget = new(Main.instance.GraphicsDevice, Size, Size, true, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
        }

        internal void ApplyThingToTarget(RenderTarget2D currentField, Action shaderPreparationsAction)
        {
            Main.instance.GraphicsDevice.SetRenderTarget(TemporaryAuxilaryTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

            shaderPreparationsAction();
            Main.spriteBatch.Draw(currentField, currentField.Bounds, Color.White);
            Main.spriteBatch.End();

            currentField.CopyContentsFrom(TemporaryAuxilaryTarget);
        }

        internal void FlushQueueToTarget(FluidFieldState field)
        {
            ApplyThingToTarget(field.NextState, () =>
            {
                int batchIndex = 0;
                int pixelCount = field.PendingChanges.Count;

                // Get the FUCK out of here if the queue is empty. If this check isn't here the primitive drawing method will attempt to access
                // memory that it shouldn't and the OS will tell the program to go fuck itself, resulting in a crash.
                if (pixelCount <= 0)
                    return;

                Texture2D pixel = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Pixel").Value;
                CalamityUtils.CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix);
                BasicShader.View = viewMatrix;
                BasicShader.Projection = projectionMatrix;

                // Go through all particles and draw them directly with primitives.
                BasicShader.CurrentTechnique.Passes[0].Apply();

                // Decide the vertices and indices.
                FieldVertex2D[] vertices = new FieldVertex2D[pixelCount * 4];
                short[] indices = new short[pixelCount * 6];

                // Go through the queue and prepare the vertices/indices.
                while (field.PendingChanges.TryDequeue(out PixelQueueValue v))
                {
                    Color value = new(v.Value.X, v.Value.Y, v.Value.Z, v.Value.W);
                    Vector2 topLeft = v.Position;
                    Vector2 topRight = v.Position + Vector2.UnitX;
                    Vector2 bottomLeft = v.Position + Vector2.UnitY;
                    Vector2 bottomRight = v.Position + Vector2.One;

                    vertices[batchIndex * 4] = new FieldVertex2D(topLeft, v.Value, new Vector2(0f, 0f));
                    vertices[batchIndex * 4 + 1] = new FieldVertex2D(topRight, v.Value, new Vector2(1f, 0f));
                    vertices[batchIndex * 4 + 2] = new FieldVertex2D(bottomRight, v.Value, new Vector2(1f, 1f));
                    vertices[batchIndex * 4 + 3] = new FieldVertex2D(bottomLeft, v.Value, new Vector2(0f, 1f));

                    // Construct independent primitives by creating a square from two triangles defined by the edges of the particle.
                    indices[batchIndex * 6] = (short)(batchIndex * 4);
                    indices[batchIndex * 6 + 1] = (short)(batchIndex * 4 + 1);
                    indices[batchIndex * 6 + 2] = (short)(batchIndex * 4 + 2);
                    indices[batchIndex * 6 + 3] = (short)(batchIndex * 4);
                    indices[batchIndex * 6 + 4] = (short)(batchIndex * 4 + 2);
                    indices[batchIndex * 6 + 5] = (short)(batchIndex * 4 + 3);
                    batchIndex++;

                    // I don't know why this is necessary to make the primitives render, but it is.
                    Main.spriteBatch.Draw(pixel, Vector2.Zero, null, Color.Transparent);
                }

                // Flush the vertices and indices and draw them to the render target.
                Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, pixelCount * 4, indices, 0, pixelCount * 2);
            });
        }

        internal void CalculateDiffusion(float diffusionFactor, FluidFieldState field, bool colors = false)
        {
            diffusionFactor *= DeltaTime * Size;
            ApplyThingToTarget(field.NextState, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = field.PreviousState;
                CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                CalamityShaders.FluidShaders.Parameters["diffusionFactor"].SetValue(diffusionFactor);
                CalamityShaders.FluidShaders.Parameters["handlingColors"].SetValue(colors);
                CalamityShaders.FluidShaders.Parameters["dissipationFactor"].SetValue(DissipationFactor);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["DiffusionPass"].Apply();
            });
        }

        internal void CalculateAdvection(RenderTarget2D currentField, RenderTarget2D nextField, RenderTarget2D horizontalVelocities, RenderTarget2D verticalVelocities, bool colors = false)
        {
            ApplyThingToTarget(nextField, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = currentField;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                CalamityShaders.FluidShaders.Parameters["deltaTime"].SetValue(DeltaTime);
                CalamityShaders.FluidShaders.Parameters["handlingColors"].SetValue(colors);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["AdvectionPass"].Apply();
            });
        }

        internal void ClearDivergence(RenderTarget2D horizontalVelocities, RenderTarget2D verticalVelocities, RenderTarget2D p)
        {
            for (int i = 0; i < GaussSeidelIterations; i++)
            {
                ApplyThingToTarget(p, () =>
                {
                    Main.instance.GraphicsDevice.Textures[1] = p;
                    Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                    Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                    CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                    CalamityShaders.FluidShaders.CurrentTechnique.Passes["PerformPoissonIterationPass"].Apply();
                });
            }

            ApplyThingToTarget(verticalVelocities, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[4] = p;
                CalamityShaders.FluidShaders.Parameters["horizontalCase_Divergence"].SetValue(false);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["ClearDivergencePass"].Apply();
            });

            ApplyThingToTarget(horizontalVelocities, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[4] = p;
                CalamityShaders.FluidShaders.Parameters["horizontalCase_Divergence"].SetValue(true);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["ClearDivergencePass"].Apply();
            });
        }

        internal void Update()
        {
            // Everything here involves heavy manipulation of render targets to work. Doing any of that on the server
            // would certainly result in a crash due to a lack of a graphics device.
            if (Main.netMode == NetmodeID.Server)
                return;

            if (!ShouldUpdate)
                return;

            ShouldUpdate = false;
            UpdateAction?.Invoke();
            UpdateAction = null;

            // Clear queues.
            FlushQueueToTarget(HorizontalFieldSpeed);
            FlushQueueToTarget(VerticalFieldSpeed);
            FlushQueueToTarget(ColorField);
            FlushQueueToTarget(DensityField);

            UpdateVelocityFields();
            UpdateDensityFields();

            ShouldSkipDivergenceClearingStep = false;
        }

        internal void UpdateVelocityFields()
        {
            CalculateDiffusion(Viscosity, HorizontalFieldSpeed);
            CalculateDiffusion(Viscosity, VerticalFieldSpeed);

            if (!ShouldSkipDivergenceClearingStep)
                ClearDivergence(HorizontalFieldSpeed.NextState, VerticalFieldSpeed.NextState, HorizontalFieldSpeed.PreviousState);

            CalculateAdvection(HorizontalFieldSpeed.NextState, HorizontalFieldSpeed.PreviousState, HorizontalFieldSpeed.PreviousState, VerticalFieldSpeed.PreviousState);
            CalculateAdvection(VerticalFieldSpeed.NextState, VerticalFieldSpeed.PreviousState, HorizontalFieldSpeed.PreviousState, VerticalFieldSpeed.PreviousState);

            if (!ShouldSkipDivergenceClearingStep)
                ClearDivergence(HorizontalFieldSpeed.NextState, VerticalFieldSpeed.NextState, HorizontalFieldSpeed.PreviousState);
        }

        internal void UpdateDensityFields()
        {
            CalculateDiffusion(DiffusionFactor, DensityField);
            DensityField.SwapState();
            CalculateAdvection(DensityField.PreviousState, DensityField.NextState, HorizontalFieldSpeed.NextState, VerticalFieldSpeed.NextState);

            CalculateDiffusion(DiffusionFactor, ColorField, true);
            ColorField.SwapState();
            CalculateAdvection(ColorField.PreviousState, ColorField.NextState, HorizontalFieldSpeed.NextState, VerticalFieldSpeed.NextState, true);
        }

        public void Dispose()
        {
            // Prevent disposing twice.
            if (Disposing)
                return;

            FluidFieldManager.Fields.Remove(this);
            Disposing = true;

            TemporaryAuxilaryTarget?.Dispose();
            HorizontalFieldSpeed?.Dispose();
            VerticalFieldSpeed?.Dispose();
            ColorField?.Dispose();
            DensityField?.Dispose();
        }

        public void CreateSource(int x, int y, float density, Color color, Vector2 velocity)
        {
            Vector2 pos = new(x, y);

            if (x < 0 || y < 0 || x >= Size || y >= Size)
                return;

            ColorField.PendingChanges.Enqueue(new PixelQueueValue(pos, color));

            HorizontalFieldSpeed.PendingChanges.Enqueue(new(pos, new Vector4(velocity.X, 0f, 0f, 0f)));
            VerticalFieldSpeed.PendingChanges.Enqueue(new(pos, new Vector4(velocity.Y, 0f, 0f, 0f)));

            DensityField.PendingChanges.Enqueue(new PixelQueueValue(pos, new Color(density, 0f, 0f)));
        }

        public void Draw(Vector2 drawPosition, bool needsToCallEnd, Matrix drawPerspective, Matrix previousPerspective)
        {
            if (needsToCallEnd)
                Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, drawPerspective);
            Main.instance.GraphicsDevice.Textures[5] = ColorField.NextState;
            CalamityShaders.FluidShaders.CurrentTechnique.Passes["DrawFluidPass"].Apply();
            Main.spriteBatch.Draw(DensityField.NextState, drawPosition, null, Color.White, 0f, DensityField.NextState.Size() * 0.5f, Scale, 0, 0f);
            Main.spriteBatch.End();

            if (needsToCallEnd)
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, previousPerspective);
        }
    }
}
