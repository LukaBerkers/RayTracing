=======================
||| Student Details |||
=======================

Student Name: Luka Berkers
Student ID: 6793592
Student Name: Aryan Pokharna 
Student ID: 7895488

============================
||| Minimum Requirements |||
============================

A statement about what minimum requirements and bonus assignments you have implemented (if any) 
and information that is needed to grade them, including detailed information on your implementation.

----------
| Camera |
----------
The Camera class is defined in the source file "Camera.cs" which contains the code required for setting up and 
manipulating a virtual camera for ray tracing purposes. The class has the Vector3 attributes "_lookAt", "_right", "_up",
and "Position" along with a ScreenPlane struct. The Camera is instantiated within the RayTracer class, with the Camera 
constructor taking in four parameters:
1. The position of the camera in 3D space. In this case, it is Vector3.Zero, which represents the origin (0, 0, 0) in the world coordinate system.
2. The direction the camera is looking at. In this case, it is -Vector3.UnitZ, which represents the negative z-axis direction. It means the camera is looking towards the negative z-axis direction.
3. The up vector that determines the camera's orientation. In this case, it is Vector3.UnitY, which represents the positive y-axis direction. It means the camera's up direction is pointing towards the positive y-axis direction.
4. The aspect ratio of the camera's view. It is used to calculate the width and height of the camera's screen plane. 
The aspect ratio is the ratio of the width to the height of the display or viewport where the camera's view will be rendered.
By default this aspect ratio is equal to 1. 

The ScreenPlane struct represents the camera's screen plane in 3D space. It defines four corner points (TopLeft, TopRight, BottomLeft, BottomRight) that form a rectangle on the screen.
To calculate the the corner of the screen plane, for example "TopLeft" corner we can take the cross product of the lookAt and up vectors, multiply it by the aspect ratio to get a scaled value, then add this value along with the position + lookAt + up to get the corner. Adequate adjustments are completed for the other corners. 
The Camera class also calculates the screen plane based on the camera's position, look-at direction, up vector, and aspect ratio.
The LookAt and Up methods have getters and setters that updated on changes such as keyboard movement which is described later on.

-------------
| Primitives |
-------------
Primitives are defined as an abstract class with class attributes "Color" which is a RGB vector from 0 to 1 and 
"Material" which is of type MaterialType which is an enum for Matte, Plastic, and Metal.

There are three primitive types: Plane, Sphere and Triangle.

The plane primitive has attributes "normal" which is assumed to be of unit length, "distance" which is the signed
distance according to the direction of the "normal", and the class attributes of "color" and "material", with the 
default material type being matte.

The intersection for the plane is calculated... 

----------
| Lights |
----------
A light source in the ray tracing scene is represented and defined by the "Light" class with Vector3 attributes "Location" and "Intensity". 
The Light class provides a constructor that takes the location and intensity of the light source as parameters.
The Location field represents the position of the light source in 3D space. It is a Vector3 that specifies the coordinates of the light source.
The Intensity field represents the intensity of the light source, specifying its color and brightness. It is also a Vector3 that defines the RGB components of the light source.
Lights are instantiated in the MyApplication class, where "an arbitrary number of point lights" can be added by including their location and intensity in a list. 
Take for example: var lights = new List<Light> { new((3, 4, 0), (100, 24, 24)), new((-1, 5, -1), (24, 8, 8)) };

-------------
| Materials |
-------------



-----------------------
| Demonstration Scene |
-----------------------
After including the necessary source code files (.cs), dependencies (.glsl) and 
other external libraries, frameworks, and packages that the project relies on 
within the working directory, we can set the build project configuration, finally, 
running the compiler to create an executable. 

The application can be executed by running this executable at 
"/RiderProjects/RayTracing/RayTracing/bin/Debug/net7.0/RayTracing".
Since our development was done using the JetBrains Rider IDE, executing our program was simple as clicking "run" 
which would run the executable using the preset configuration. The target framework is net7.0. 

---------------
| Application |
---------------


----------------
| Debug Output |
----------------
The debug output has been implemented and can be executed by ...
Within the debug output we can see the following: sphere and 
triangle primitives, primary rays, shadow rays and secondary rays.

=====================================
||| Bonus Assignments Implemented |||
=====================================
-


===============================
||| External Materials Used |||
===============================
As of our current submission, no external materials were used.