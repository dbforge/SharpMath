# SharpMath
SharpMath is a C# math library supporting vectors, matrices, linear equation systems, expression parsing and soon more advanced analysis.
Specials thanks go to [NikxDa](https://github.com/nikxda) who wrote the Canvas2D control for representing vectors and functions graphically.

## Overview
![Class Diagramm](https://github.com/ProgTrade/SharpMath/blob/master/SharpMath.png)

## 2D and 3D-Geometry
SharpMath offers many structs, such as points, vectors, lines and matrices. These are basically also used in games/computer graphics, except that they're a bit more extensive and sophisticated. Thus, the game engines provide some more flexibility and possibilites than SharpMath.
Nevertheless, you can perform simple operations (such as rotating, scaling or translating objects defined through vertices) in space and  project everything onto a two-dimensional surface.

![3D-Sample](https://trade-programming.de/pixelkram/sharpmath_projection.png)

## Linear equation systems
SharpMath can solve linear equation systems using the Gauss-Jordan algorithm internally by representing the equations as matrices.

## Expression Parsing
SharpMath knows most of the functions, constants and operators used in computations. You can provide the parser with an input string containing your expression and it will return the result when calling the corresponding function. The flexibility of the parser allows you to add further operations. This feature may be used for calculators.

![Calculator](https://trade-programming.de/pixelkram/vstiebqhlj.png)

## Canvas2D
SharpMath provides a two-dimensional canvas control for representing vectors and functions graphically in your application. It was written by [NikxDa](https://github.com/nikxda).
A three-dimensional representation (e.g. for vectors, points and lines) will follow as well. Thanks to the  `Geometry`-namespace, the implementation of this one should not be too hard as the projection onto a two-dimensional surface can be done using the integrated matrices.
![Canvas2D](https://github.com/ProgTrade/SharpMath/blob/master/Canvas2D.png)
