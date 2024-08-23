# Boids in MonoGame

I built this as my first foray into making things with MonoGame, it's my go-to
project for something new as the elements involved are very simple, basically
just some vector math and drawing triangles onto the screen. Plus I've done it
before in other languages / frameworks / engines.

## Boids

I won't go too deep into the details here, you can read about the original boids
simulation by Craig Reynolds [here](https://cs.stanford.edu/people/eroberts/courses/soco/projects/2008-09/modeling-natural-systems/boids.html).

There is an excellent implementation walkthrough (in Unity) by Sebastian Lague
on YouTube [here](https://www.youtube.com/watch?v=bqtqltqcQhw).

## Setup

To set this repo up I basically followed the MonoGame getting started docs which
you can find [here](https://docs.monogame.net/articles/getting_started/index.html). I'm using Windows 11 so I did the follwing:

- Download and install the .NET 8 SDK (specifically I have 8.0.401)
- Install MonoGame templates with this powershell command

`dotnet new install MonoGame.Templates.CSharp`

- Run the command to setup a new MonoGame project

`dotnet new mgdesktopgl`