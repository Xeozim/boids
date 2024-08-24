using System;
using Microsoft.Xna.Framework;

namespace boids;

public class Boid
{
    // Pos and Vel can only be set by the boid itself
    private Vector2 position;
    public Vector2 Position { get => position; }
    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    public float Heading {
        get => (float)Math.Atan2(Velocity.X, -Velocity.Y);
    }

    public Boid(Vector2 position, Vector2 velocity)
    {
        this.position = position;
        this.velocity = velocity;
    }

    // Update the position and velocity of the boid
    // dt is the amount of time that passed since the last update in seconds
    public void Update(float dt)
    {
        position += velocity * dt;
    }
}