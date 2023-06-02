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
The Camera class fulfills the requirements for supporting arbitrary field of view, positions, and orientations.
1. Arbitrary Field of View (FOV): The Camera class allows setting the field of view (FOV) through the RayTracer constructor, which takes an aspect ratio parameter (var aspectRatio = (float)display.Width / display.Height;). By providing the desired aspect ratio, we can control the width-to-height ratio of the camera's viewing plane. This indirectly affects the FOV, as a wider aspect ratio will result in a wider field of view, and a narrower aspect ratio will result in a narrower field of view. In the Camera class we clamp the FOV between 60 and 90 degrees, and to calculate the scalar by which we multiply the lookAt vector, we execute this mathematical operation: float scalar = 1 / (MathF.Tan(MathF.PI * clampedDegree / 180.0f / 2.0f)); effectively changing the FOV within the degree domain. 
2. Arbitrary Positions and Orientations: The Camera class allows setting the camera's position and orientation through its constructor. The position parameter defines the camera's location in 3D space, while the lookAt and up parameters determine the camera's orientation and where it is pointing.
    * Position: The position parameter allows specifying an arbitrary 3D position for the camera. This enables placing the camera at any desired location within the scene.
    * LookAt: The lookAt parameter specifies the target or point in 3D space that the camera is aimed at. By providing an arbitrary lookAt vector, we can control the camera's orientation and define where it is directed.
    * Up: The up parameter defines the camera's up direction, which is used to determine the camera's tilt and roll. By specifying an arbitrary up vector, we can control the camera's orientation and achieve different viewing angles.
The Camera class also provides getter and setter properties for the lookAt and up vectors, allowing us to dynamically update the camera's orientation during runtime.
In summary, the Camera class supports arbitrary field of view by allowing specification of the aspect ratio. It also supports arbitrary positions and orientations by accepting parameters for the camera's position, lookAt target, and up direction. This enables flexibility in placing and aiming the camera within the 3D scene.

High-level code overview:
The Camera class is defined in the source file "Camera.cs" which contains the code required for setting up and 
manipulating a virtual camera for ray tracing purposes. The class has the Vector3 attributes "_lookAt", "_right", "_up",
and "Position" along with a ScreenPlane struct. The Camera is instantiated within the RayTracer class, with the Camera 
constructor taking in four parameters:
1. The position of the camera in 3D space. By default, it is Vector3.Zero, which represents the origin (0, 0, 0) in the world coordinate system.
2. The direction the camera is looking at. By default, it is -Vector3.UnitZ, which represents the negative z-axis direction. It means the camera is looking towards the negative z-axis direction.
3. The up vector that determines the camera's orientation. By default, it is Vector3.UnitY, which represents the positive y-axis direction. It means the camera's up direction is pointing towards the positive y-axis direction.
4. The aspect ratio of the camera's view. It is used to calculate the width and height of the camera's screen plane. The aspect ratio is the ratio of the width to the height of the display or viewport where the camera's view will be rendered. By default this aspect ratio is equal to 1. 

The ScreenPlane struct represents the camera's screen plane in 3D space. It defines four corner points (TopLeft, TopRight, BottomLeft, BottomRight) that form a rectangle on the screen. To calculate the the corner of the screen plane, for example "TopLeft" corner we can take the cross product of the lookAt and up vectors, multiply it by the aspect ratio to get a scaled value, then add this value along with the position + lookAt + up to get the corner. Adequate adjustments are completed for the other corners. The Camera class also calculates the screen plane based on the camera's position, look-at direction, up vector, and aspect ratio. The LookAt and Up methods have getters and setters that updated on changes such as keyboard movement which is described later on.

--------------
| Primitives |
--------------
Primitives are defined as an abstract class with class attributes "Color" which is a RGB vector from 0 to 1 and 
"Material" which is of type MaterialType which is an enum for Matte, Plastic, and Metal.

Plane:
The plane primitive has attributes "normal" which is assumed to be of unit length, "distance" which is the signed
distance according to the direction of the "normal", and the class attributes of "color" and "material", with the 
default material type being matte.

The code defines a class called Plane that inherits from the Primitive class. This class represents a plane in 3D space.
1. It has additional properties for Normal (representing the plane's normal vector) and Distance (representing the signed distance of the plane from the origin).
2. It overrides the Intersect method to calculate the intersection between a given Ray and the plane.
3. The intersection calculation (as derived from the mathematical equations discussed in lectures) involves dot product operations and other equations to determine the intersection point and whether the ray is parallel to the plane.

Sphere:
The code defines a class called Sphere that also inherits from the Primitive class. This class represents a sphere in 3D space.
1. It has additional properties for Position (representing the center position of the sphere) and Radius (representing the sphere's radius).
2. It has a constructor that initialises the Position, Radius, Color, and Material properties of the sphere (similar to the Plane primitive).
3. It overrides the Intersect method to calculate the intersection between a given Ray and the sphere.
4. The intersection calculation involves quadratic formula computations and dot product operations to determine the intersection points and surface normal of the sphere.

These classes provide the necessary functionality to perform intersection tests between rays and the primitives planes and spheres, allowing for accurate rendering of the scene and interaction with light sources and materials.

---------
| Scene |
---------
This Scene class provides a way to organise and manage the objects in the scene, allowing for flexible scene definition by adding and removing light sources and primitives as needed. 
It provides a convenient method to find the closest intersection between a ray and the scene's primitives, which is crucial for ray tracing calculations.
To do so the class has two public properties: LightSources and Primitives which hold instances of the Light and Primitive objects, respectively.
The ClosestIntersection method takes a Ray as input and returns the closest intersection between the ray and any primitive in the scene. 
It iterates over each primitive in the Primitives list and calls the Intersect method of each primitive to check for intersections with the ray. 
It keeps track of the closest intersection found so far by comparing the distances of the intersections. The method returns the closest intersection found or null if no intersection occurs.

----------
| Lights |
----------
A light source in the ray tracing scene is represented and defined by the "Light" class with Vector3 attributes "Location" and "Intensity". 
The Light class provides a constructor that takes the location and intensity of the light source as parameters.
The Location field represents the position of the light source in 3D space. It is a Vector3 that specifies the coordinates of the light source.
The Intensity field represents the intensity of the light source, specifying its color and brightness. It is also a Vector3 that defines the RGB components of the light source.
Lights are instantiated in the MyApplication class, where "an arbitrary number of point lights" can be added by including their location and intensity in a list. 
Take for example: var lights = new List<Light> { new((3, 4, 0), (100, 24, 24)), new((-1, 5, -1), (24, 8, 8)) }; which after execution contains two light objects, each representing a light source with a specific position and intensity. These light sources can be used in the rendering or illumination calculations within the ray-tracing application.

-------------
| Materials |
-------------
To support the Phong shading model, each primitive (planes and spheres) in the Primitive class can be assigned a material type, such as Matte, Plastic, or Metal, which are defined as enum values in the code.
The material type can be used to determine the shading calculations for each intersection point, considering factors like diffuse reflection, specular reflection, and ambient light.

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
Keyboard movement is observed from the OnUpdateFrame method within “Template.cs”. 
The method checks the state of specific keyboard keys (W, A, S, D) and stores the corresponding boolean values (true if the key is pressed, false otherwise) in the variables wPressed, aPressed, sPressed, and dPressed.
Subsequently, it calls the Update method of the _app object and passes the boolean values as arguments. This allows the application to handle the keyboard inputs and update its internal state based on the pressed keys.
The Update method is responsible for updating the camera placement based on keyboard movement inputs. The method begins by defining a constant moveSpeed with a float value of 0.2, which determines the speed at which the camera moves or rotates. Next, it checks if the 'W' key is pressed (wPressed is true). If so, it calls the _rayTracer.MoveCameraForward method and passes the moveSpeed as an argument, effectively moving the camera forward in the scene. Similar function calls occur for the other keyboard movements - for instance moving the camera backward when the ’S’ key is pressed, and rotating the camera to the left or right when the 'A' or 'D' keys are pressed, respectively.
Note that we utilise conditional “if” and “else if” statements to ensure that the user cannot simultaneously move the camera forward and backward, or simultaneously rotate the camera left and right. 
Within “RayTracer.cs” the camera's position is updated by adding moveSpeed multiplied by the camera's LookAt vector. This effectively moves the camera in the direction it is currently looking. To ensure that we can rotate the camera on it’s y-axis, we can calculate the right vector and update the camera's position by adding moveSpeed multiplied by the right vector. This rotates the camera around its vertical axis to the right. Similar calculations can be performed to achieve backwards movement and leftwards rotation 

----------------
| Debug Output |
----------------
Within the debug output we can see the following: sphere primitives, primary rays, shadow rays and secondary rays.

The debug output has been implemented and can be executed by simply changing the boolean "debug" variable to true in the Tick() function in MyApplication.cs. This will then execute the debug method within the ray tracer class. 

A high level summary of the debug class is that we use it to visualise the camera, light sources, screen plane, spheres, and rays in the scene. 
For instance, the DebugWorldToScreen method is used to convert three-dimensional world coordinates to two-dimensional screen coordinates, by ignoring the y-plane in the 3D vector.  
Another method such as the DebugDrawCircle method is used to draw circles representing the intersection of spheres with a specific y-plane.

The primary rays are generated by iterating over a set of 11 rays. Each ray represents a path from the camera through a specific pixel on the screen. The rays are evenly distributed across the screen plane.

For each primary ray:
1. The position of the ray on the screen plane is calculated based on its index using the ratioAlongScreen value.
2. A new Ray object is created with the camera's position as the base and a normalised direction vector pointing from the camera towards the calculated screen position.
3. The closest intersection of the primary ray with objects in the scene is found using Scene.ClosestIntersection(ray).
4. If an intersection exists, the distance to the intersection point is calculated; otherwise, a default distance of 100.0 is used.
5. The intersection point in the 3D world is evaluated using the ray and distance.
6. The world-space intersection position is converted to a screen coordinate using DebugWorldToScreen().
7. A line is drawn on the display from the camera's screen position to the intersection position, visualizing the primary ray.
This process allows visualisation of the primary rays and their intersections with objects in the scene.

Shadow rays are generated to determine whether an intersection point on a primary ray is obstructed by objects in the scene from the perspective of each light source. 
The process is as follows:
1. If an intersection exists on the primary ray (intersection is not null), shadow rays are computed for each light source in the scene.
2. For each light source, a shadow ray is created from the intersection point towards the light source's location.
3. The shadow ray is normalized to ensure consistent direction and its intersection with objects in the scene is computed using Scene.ClosestIntersection(shadowRay).
4. The distance to the closest intersection along the shadow ray is calculated. If there is no intersection, a default distance of 100.0 is used.
5. The intersection point on the shadow ray is evaluated using shadowRay.Evaluate(shadowDistance) to obtain the world-space intersection position.
6. The world-space intersection position is converted to a screen coordinate using DebugWorldToScreen(shadowIntersectionLocation).
7. A line is drawn on the display from the intersection position of the primary ray to the intersection position of the shadow ray, providing a visual representation of the shadow ray.

For outputting the sphere primitive we created the DebugDrawCircle method which is used to draw a circle on the display for debugging purposes. 
It takes the center position of the circle, its radius, and the desired color as parameters. The method approximates the circle by dividing it into multiple line segments. The number of line segments is determined by the segments variable, which is set to 64 in this case.
For each segment, the method calculates the start and end angles based on the segment index. Using trigonometric calculations, it determines the start and end points of the line segment by rotating a unit vector (Vector2.UnitX) by the corresponding angles and scaling it by the radius.
To display the circle on the screen, the method converts the world-space coordinates of the start and end points to screen coordinates using the DebugWorldToScreen method. Finally, it draws a line on the display between the screen positions of the start and end points, using the specified color.
In summary, the DebugDrawCircle method provides a convenient way to visualize circles on the display by approximating them with line segments. It helps in debugging and understanding the behavior of the program.

=====================================
||| Bonus Assignments Implemented |||
=====================================

===============================
||| External Materials Used |||
===============================
Many of the calculations are implementations of formulas and algorithms form the lecture slides.
Some code in the Camera constructor is based on a section from the course literature, this is annotated with a comment.