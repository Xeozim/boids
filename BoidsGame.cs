using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace boids;

public class BoidsGame : Game
{
    private GraphicsDeviceManager _graphics;
    private BasicEffect _basicEffect;
    private VertexBuffer _vertexBuffer;

    private Boid[] _boids;

    // Limits of the world. 100x100
    private static BoundingBox worldLimits = new BoundingBox(new Vector3(-50,-50,0),new Vector3(50,50,0));
    
    // Standard non-rotated vertex position of the boid polygon
    // Split into two triangles for each half of the boid
    // Note each triangle's vertices should be in clockwise winding order 
    private static VertexPosition[] boidVertices =
    [
        new VertexPosition(new Vector3( 0.0f,-1.0f, 0)),    // Front
        new VertexPosition(new Vector3(-0.5f, 1.0f, 0)),    // Left tail point
        new VertexPosition(new Vector3( 0.0f, 0.5f, 0)),    // Back
        ////
        new VertexPosition(new Vector3( 0.0f, 0.5f, 0)),    // Back
        new VertexPosition(new Vector3( 0.5f, 1.0f, 0)),    // Right tail point
        new VertexPosition(new Vector3( 0.0f,-1.0f, 0)),    // Front
    ];

    public BoidsGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Add initialization logic here

        // Screen setup
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1280;
        _graphics.ApplyChanges();
        
        // Create a single boid at the centre of the screen with random velocity
        Random rnd = new Random();
        Vector2 vel = new Vector2((float)rnd.NextDouble()-0.5f, (float)rnd.NextDouble()-0.5f) * 10;
        _boids = [new Boid(Vector2.Zero, vel)];

        // Create the vertex buffer used for drawing the boids
        _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition), boidVertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(boidVertices);

        // Initialize the BasicEffect for rendering
        _basicEffect = new BasicEffect(GraphicsDevice)
        {
            DiffuseColor = Color.White.ToVector3(),
            View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up),
            Projection = Matrix.CreateOrthographicOffCenter(worldLimits.Min.X, worldLimits.Max.X, worldLimits.Min.Y, worldLimits.Max.Y, 0, 1)
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update logic here
        foreach(Boid boid in _boids){
            boid.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Drawing code here

        // Set the vertex buffer to the graphics device
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        // Set the vertex buffer to the graphics device
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);

        // Draw each boid by offsetting the standard set of vertices
        // No rotation for now.
        foreach(Boid boid in _boids){
            DrawBoid(boid);
        }

        base.Draw(gameTime);
    }

    private void DrawBoid(Boid boid)
    {
        // Rotate the draw effect according to the boid heading and translate to it's position
        _basicEffect.World = Matrix.CreateRotationZ(boid.Heading) * Matrix.CreateTranslation(new Vector3(boid.Position, 0));

        // Apply the BasicEffect settings
        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
        }
    }
}
