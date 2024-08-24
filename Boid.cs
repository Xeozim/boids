using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace boids;

public class Boid
{
    // Properties

    // ID used for excluding this boid when we're going through lists of all boids
    private readonly int _id;
    public int Id { get => _id; }

    // Dynamic properties
    private Vector2 _position;
    public Vector2 Position { get => _position; }
    private Vector2 _velocity;
    public Vector2 Velocity { get => _velocity; }

    //// Boid Behaviour. Currently applied to all boids the same
    // Neighbourhood definition
    // Range limit, determines how far away other boids can be and still impact
    // this boid's behaviour
    private static double _neighbourhoodRange = 10;
    public static double NeighbourhoodRange { get => _neighbourhoodRange; set => _neighbourhoodRange = value; }
    // Field of view limit (in radians), if the angle between the velocity
    // vector and the other boid position is more than this value we ignore it  
    private static double _neighbourhoodFOV = Math.PI * 0.75;
    public static double NeighbourhoodFOV { get => _neighbourhoodFOV; set => _neighbourhoodFOV = value; }

    // Rule factors
    // Separation factor, how much the boid steers away from nearby boids
    private static float _seperationFactor = 0.5f;
    public static float SeperationFactor { get => _seperationFactor; set => _seperationFactor = value; }
    // Separation range, how close a boid has to be before we try to avoid it
    private static float _seperationRange = 1.0f;
    public static float SeperationRange { get => _seperationRange; set => _seperationRange = value; }
    // Alignment factor, how much the boid steers towards the average velocity of it's neighbours
    private static float _alignmentFactor = 0.01f;
    public static float AlignmentFactor { get => _alignmentFactor; set => _alignmentFactor = value; }
    // Cohesion factor, how much the boid steers towards the average position of it's neighbours
    private static float _cohesionFactor = 0.1f;
    public static float CohesionFactor { get => _cohesionFactor; set => _cohesionFactor = value; }

    // Limitations
    private static float _speedLimit = 50;
    public static float SpeedLimit { get => _speedLimit; set => _speedLimit = value; }

    public Boid(int id, Vector2 position, Vector2 velocity)
    {
        this._id = id;
        this._position = position;
        this._velocity = velocity;
    }

    // Neighbourhood check helper
    protected bool NeighbourhoodContains(Boid other){
        Vector2 displacement = other.Position - _position;
        double phi = Math.Acos(Vector2.Dot(displacement,_velocity) / (displacement.Length() * _velocity.Length()));
        return displacement.Length() < _neighbourhoodRange && Math.Abs(phi) < _neighbourhoodFOV;
    }

    // Filter a list of boids according to the neighbourhood properties of this boid
    protected List<Boid> GetNeighbours(List<Boid> boids){
        return boids.Where(x => NeighbourhoodContains(x) && x.Id != _id).ToList();
    }

    // Update the position and velocity of the boid
    // dt is the amount of time that passed since the last update in seconds
    public void Update(float dt, List<Boid> allBoids, BoundingBox worldLimits)
    {
        // Calculate new velocity from the rules
        Vector2 newVelocity = _velocity;

        List<Boid> neighbours = GetNeighbours(allBoids);

        if (neighbours.Count != 0){
            // Move away from boids we're trying to avoid
            Vector2 seperationComponent = new();
            foreach (Boid other in neighbours){
                Vector2 displacementVector = other.Position - _position;
                float range = displacementVector.Length();
                if (range < _seperationRange){
                    displacementVector.Normalize();
                    seperationComponent -= displacementVector * (_seperationFactor / range);
                }
            }

            // Steer towards neighbours' average velocity vector
            Vector2 averageVelocity = new (neighbours.Average(b => b.Velocity.X),neighbours.Average(b => b.Velocity.Y));
            Vector2 alignmentComponent = (averageVelocity - _velocity) * _alignmentFactor;

            // Steer towards neighbours' average position
            Vector2 averagePosition = new (neighbours.Average(b => b.Position.X),neighbours.Average(b => b.Position.Y));
            Vector2 cohesionComponent = (averagePosition - _position) * _cohesionFactor;

            newVelocity += seperationComponent + alignmentComponent + cohesionComponent;
        }

        // Apply speed limit to new velocity
        if (newVelocity.Length() > _speedLimit){
            newVelocity.Normalize();
            newVelocity *= _speedLimit;
        }
        _velocity = newVelocity;

        // Apply velocity to position
        _position += _velocity * dt;

        // If we're outside of the limits of the world, teleport to the other edge
        if (_position.X > worldLimits.Max.X) {_position.X = _position.X - worldLimits.Max.X + worldLimits.Min.X; }
        if (_position.X < worldLimits.Min.X) {_position.X = _position.X - worldLimits.Min.X + worldLimits.Max.X; }
        if (_position.Y > worldLimits.Max.Y) {_position.Y = _position.Y - worldLimits.Max.Y + worldLimits.Min.Y; }
        if (_position.Y < worldLimits.Min.Y) {_position.Y = _position.Y - worldLimits.Min.Y + worldLimits.Max.Y; }
    }
}