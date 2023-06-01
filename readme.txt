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
Lights are defined by the "Light" class with Vector3 attributes "Location" and "Intensity".
The Lights are set in the 

        var lights = new List<Light> { new((3, 4, 0), (100, 24, 24)), new((-1, 5, -1), (24, 8, 8)) };


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