# Changelog

All notable changes to SharpMath are documented in this file. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project follows [Semantic Versioning](https://semver.org/).

## [2.0.0] - Unreleased

SharpMath 2.0 is a rewrite for .NET 10. Almost every public type changed. The sections below double as a migration
guide from 1.x.

### Breaking changes

#### Platform and packaging

- Targets `net10.0` (C# 14) instead of .NET Framework 4.0. The WinForms parts target `net10.0-windows`.
- The library is split into five assemblies: `SharpMath`, `SharpMath.Canvas`, `SharpMath.Canvas.Skia`,
  `SharpMath.Canvas.WinForms` and `SharpMath.Canvas.Avalonia`. The core no longer references WinForms or System.Drawing.
- Removed `[Serializable]`, `[assembly: ComVisible(true)]` and the GUID. `BinaryFormatter` no longer exists in .NET.
- Namespaces:
  - `SharpMath.Presentation` → `SharpMath.Canvas`
  - `SharpMath.Equations.Exceptions` → `SharpMath.Equations`
  - `SharpMath.Expressions.Exceptions` → `SharpMath.Expressions`

#### Value types are immutable

- `Vector2/3/4`, `Point2D/3D`, `Matrix2x2/3x3/4x4`, `Line2D`, `Line3D`, `LineSegment2D`, `BoundingBox2D` and
  `ProjectionData` are `readonly struct`s. Settable properties and indexers are gone. Use the constructors or factory
  methods instead of `new Matrix2x2 { [0, 0] = 8, … }`. Vectors, points, matrices and segments support `with`
  (`v with { X = 1 }`, `m with { M14 = 5 }`); lines, bounding boxes and projection data validate or normalize their
  values and are created through their constructors or factories only.
- Indexers take `int` instead of `uint` and are read-only. They throw `ArgumentOutOfRangeException` (was
  `IndexOutOfRangeException`), and the 2D indexers of matrices now check row and column separately.
- Vectors, points and matrices no longer implement `IEnumerable<double>`. Use the indexers or the properties.
- `Polygon` is immutable and no longer implements `IEnumerable<Point2D>`. `Points` is a `ReadOnlyCollection<Point2D>`.
- `LinearEquation` and `LinearEquationSystem` are immutable. `LinearEquation` has a single constructor,
  `new LinearEquation([a₁, a₂, …], result)`. The parameterless constructors and `LinearEquation(double result, params
  double[] coefficients)` were removed.
- `Polygon`, `LinearEquation` and `LinearEquationSystem` are `sealed`.
- `Dimension`, `RowCount` and `ColumnCount` are `int`.
- `Equals(object)` of a vector no longer accepts other `IVector` types.

#### Renamed and reshaped members

| 1.x | 2.0 |
|---|---|
| `Magnitude`, `SquareMagnitude` | `Length`, `LengthSquared` |
| `DotProduct` | `Dot` |
| `VectorProduct` | `Cross` (`Vector3` → `Vector3`, `Vector2` → `double`) |
| `CheckForOrthogonality`, `CheckForOrthonormality`, `CheckForParallelism` | `IsOrthogonalTo`, `IsOrthonormalTo`, `IsParallelTo` |
| `Vector2.AreOrthogonal/AreOrthonormal/AreParallel(a, b)` | `a.IsOrthogonalTo(b)` etc. |
| `Vector2.Area`, `Vector3.Area` | `ParallelogramArea` |
| `Vector2(Point2D bottom, Point2D tip)` | `tip - bottom` |
| `Point2D.PositionVector`, `new Point2D(Vector2)` | `point.ToVector()`, `vector.ToPoint()` |
| `Vector2.Divide(vector, scalar)` | `vector / scalar` |
| `vector.Transform(matrix)`, `Vector2.Transform(matrix, vector)` | `matrix * vector` or `Vector2.Transform(vector, matrix)` |
| `Vector3.FromVector(IVector)`, `Convert<T>()` | constructors, e.g. `new Vector3(v2, z)`, `new Vector4(v3, w)`, `Vector3.FromHomogeneous(v4)` |
| `vector.Negate()` | `-vector` |
| `Point2D + Point2D` | `Point2D + Vector2` (point − point is a vector) |
| `Vector2 * Vector2` (dot product) | `Dot` |
| `Matrix3x3/4x4.Scalation` | `Scaling` |
| `Inverse`, `Transpose`, `Adjugate`, `CofactorMatrix` (properties) | `Inverse()`, `Transpose()`, `Adjugate()`, `Cofactors()`; plus `TryInvert(out m)` |
| `Negate` (matrix property) | unary `-` |
| `IsTriangle` | `IsTriangular` |
| `IsAntiSymmetric` | `IsSkewSymmetric` (they were identical) |
| `Matrix4x4.View` | `Matrix4x4.LookAt` |
| `vector * matrix` | `matrix * vector` (column vectors, see below) |
| `new Line2D(slope, offset)`, `Offset` | `Line2D.FromSlopeIntercept(slope, yIntercept)`, `YIntercept` |
| `new Line2D(pointA, pointB)` | `Line2D.FromPoints(pointA, pointB)` |
| `Line2D.GetPoint(x)` | `SolveForY(x)` |
| `IsPointOnLine` | `Contains` |
| `IntersectsWith`, `AreIntersecting`, `GetIntersectionPoint` | `Intersect` returning `LineIntersection2D`/`LineIntersection3D` with a `Kind` |
| `Line2D.AreParallel(a, b)` | `a.IsParallelTo(b)` |
| `new Line3D(point, direction)` | `Line3D.FromPointAndDirection(point, direction)` |
| `Line3D.GetPoint(t)` | `PointAt(distance)` (the direction is now a unit vector) |
| `Polygon.ContainsPoint` | `Polygon.Contains` |
| `Polygon.Center` | `Polygon.Centroid` |
| `Polygon.BoundingBox` (`RectangleF`) | `BoundingBox2D` |
| `ProjectionData(Size, float aspect, float fov, float near, float far)` | `ProjectionData(double width, double height, double aspectRatio, double fieldOfView, double nearPlane, double farPlane)` |
| `new Parser(text).Evaluate()` | `MathExpression.Parse(text).Evaluate()` |
| `FloatingNumber`, `FloatingNumberExtensions` | `Tolerance` |
| `MathHelper.Pi`, `E`, `TwoPi`, `DegreesToRadians`, … | `Math.PI`, `Math.E`, `Math.Tau`, `double.DegreesToRadians`, `double.RadiansToDegrees` |
| `EquationNotSolvableException` for singular `Inverse` | `InvalidOperationException` |
| `DimensionException` | `ArgumentException` |

#### Removed

- The unfinished `SharpMath.Analysis` namespace (it was never compiled), all commented-out code, and unused helpers.
- `Matrix1x1`, `Matrix1x3`, `Matrix3x1`, `Matrix1x4`, `Matrix4x1`, `ISquareMatrix<T>`, `MatrixUtils`, `VectorUtils`,
  `MatrixEnumerator`, `AsHorizontalMatrix`, `AsVerticalMatrix`. `IVector`, `IMatrix` and `ISquareMatrix` remain as
  read-only interfaces.
- `Algorithms` (shunting yard), `Token`, `Token<T>`, `TokenType`, `TokenEx`, `CharEx` and the `protected static`
  operator dictionaries. Extend the parser with an `ExpressionContext` instead.
- `MathHelper`.

#### Canvas

- The canvas is no longer a WinForms control. `SharpMath.Canvas.Canvas2D` is a platform-neutral model, and
  `Canvas2DControl` (WinForms) and `Canvas2DView` (Avalonia) display it through their `Canvas` property.
- `Functions` (`BindingList<FunctionWrapper>`) and `Vectors` (`BindingList<Vector2>`) became `Items`
  (`ObservableCollection<CanvasItem>`) with `AddFunction`, `AddVector`, `AddPoint`, `AddLine`, `AddSegment` and
  `AddPolygon`. `FunctionWrapper` was removed.
- `GridOrigin`, `GridSize` and `GridScale` became `Viewport` (`Origin`, `ScaleX`, `ScaleY`, `SetOrigin`, `SetScale`,
  `Pan`, `ZoomAt`).
- `TrackingLines` and `ValueIndicator` became `ShowTrackingLines` and `ShowValueIndicator`.
- Colors, fonts and line widths are no longer hard-coded but configured through `CanvasStyle` and `ItemStyle`.

### Behaviour changes

These follow the rule "mathematical definition first, then `System.Numerics`, then least surprise".

- **Matrix convention:** column vectors everywhere (`v' = M · v`). `Matrix4x4.LookAt`, `World` and
  `PerspectiveFieldOfView` used a row-vector (Direct3D) layout and are now transposed. The `Matrix3x3`/`Matrix4x4`
  constructors that take vectors place them as rows, as documented (they used to become columns). The projection
  stays right-handed with a depth range of [0, 1], and its `M44` is now 0.
- `Vector3.Forward` is (0, 0, −1) and `Vector3.Back` is (0, 0, 1), matching the right-handed convention.
- Equality is exact on every type and follows `System.Numerics`: `==` compares components with IEEE semantics (NaN is
  not equal to NaN), `Equals` treats NaN as equal and is consistent with `GetHashCode`. Use `ApproximatelyEquals`
  (new on vectors, points, matrices, lines, segments, polygons and bounding boxes) for tolerance-based comparisons.
- Predicates no longer use one absolute epsilon of `1.6e-9`:
  - `IsIdentity`, `IsZero`, `IsNormalized` and the `Contains` checks use a combined tolerance (absolute `1e-12`,
    relative `1e-9`).
  - `IsDiagonal`, `IsTriangular`, `IsSymmetric` and `IsSkewSymmetric` compare entries relative to the largest entry,
    and `IsOrthogonal` compares `M · Mᵀ` with the identity.
  - `IsParallelTo` and `IsOrthogonalTo` compare the sine or cosine of the angle with `1e-9`, independent of the vector
    lengths.
  - `IsSingular` uses an LU decomposition with partial pivoting: a matrix is singular if a pivot is at most
    `n · ε · s` (`s` = largest absolute entry), the same rule `MatrixMxN` uses. Scaled identities such as
    `0.001 · I` and transformations with large translations are regular; matrices whose rows differ by many orders of
    magnitude can be numerically singular.
- `Inverse()` throws for singular matrices; `TryInvert` does not throw.
- `IsDiagonal` and `IsTriangular` follow the mathematical definitions: a zero on the diagonal does not matter.
- The zero vector counts as parallel and orthogonal to every vector.
- `Line3D` is stored canonically: `Point` is the point of the line closest to the origin and `Direction` is a unit
  vector whose first non-zero component is positive, so `Line3D.FromPoints(p, q).Point` is generally not `p`.
- `ToString()` uses the invariant culture everywhere; matrices print as `Matrix2x2 [[1, 2], [3, 4]]`.
- `Matrix3x3 * Vector2` divides by the resulting W component if it is not 1 (projective transformations).
- `ProjectionData` requires a field of view below π and a positive aspect ratio and throws
  `ArgumentOutOfRangeException`.
- `Vector3.ScalarTripleProduct` returns the signed value `a · (b × c)`; it used to return the absolute value.
- `Normalize()` of the zero vector throws `InvalidOperationException` (it returned NaN). It also normalizes very small
  and very large vectors correctly.
- `MoveTowards` stops at the target instead of overshooting.
- `LinearEquationSystem.Solve` requires exactly one coefficient per equation and throws `ArgumentException` otherwise.
  Too many coefficients used to throw `IndexOutOfRangeException`, and too few were silently padded with zeros.
- Expressions:
  - `!` is the factorial; it used to be the parser's internal negation marker.
  - Functions require parentheses.
  - Letters that are not functions or constants are variables.
  - Multiplication may be implicit.
  - Every syntax error is a `ParserException` with a `Position`, including empty input (it was `System.Exception`)
    and unbalanced brackets (they were `InvalidOperationException` or silently ignored).
  - Two or more letters directly in front of `(` must name a function (or end with a known constant or declared
    variable), so `cot(x)` reports an unknown function instead of multiplying `c·o·t·x`. Write `x*y*(2)` or `x y(2)`
    for products of single-letter variables.
  - Input nested more than 256 levels deep or with more than 4096 nested operations is rejected with a
    `ParserException` instead of overflowing the stack.
- Canvas: axis labels use the invariant culture on both axes; ticks use "nice" steps (1, 2, 5 × 10ⁿ) and labels are
  always placed on grid lines; labels that would be cut off at the edges are skipped; the value indicator shows as many
  decimals as the zoom level needs; zooming is limited to 10⁻⁶–10⁹ pixels per unit.

### Fixed

- `Matrix2x2/3x3/4x4.Inverse` returned the identity matrix, because the Gauss-Jordan helpers mutated boxed copies.
  Singular matrices also returned the identity.
- Gauss-Jordan searched for the pivot in the wrong cell and did not pivot partially, so some regular systems were
  reported as unsolvable.
- `Matrix2x2.IsIdentity` checked `M22 == 0`. `Matrix3x3/4x4.IsIdentity` ignored off-diagonal entries (and `M44`).
- `MatrixEnumerator` enumerated in the wrong order and threw for 1x3/1x4 matrices.
- `Matrix1x1` equality was approximate while its hash code was exact.
- `IsOrthogonal`, `IsSymmetric` and `IsSkewSymmetric` compared exactly, so real rotation matrices were "not orthogonal".
- `Matrix1x1.CofactorMatrix` and `Adjugate` threw.
- `Vector3.Transform` divided Z by W twice.
- `Matrix4x4.View` lost the camera translation and did not normalize its basis. `World` lost the position and ignored
  the results of `Normalize`.
- `vector * matrix` silently computed `matrix * vector`.
- `ProjectionData` did not validate the aspect ratio, and the field-of-view error message mentioned 360°.
- `AsHorizontalMatrix`/`AsVerticalMatrix` accepted wrong target types.
- `Vector4.UnitW` was (0, 0, 1, 0).
- `CheckForParallelism` returned false whenever a component was zero.
- `Angle` returned NaN for (nearly) parallel vectors.
- `MoveTowards` returned NaN for equal vectors.
- `Line3D.IsPointOnLine` was wrong for directions with zero components.
- `Line2D` could not represent vertical lines, and coincident lines reported no intersection. Refs #9: lines can now
  be created from the general form `Ax + By + C = 0`.
- Comparing two different polygons with `==` crashed with a stack overflow.
- `Polygon.GetHashCode` differed for equal polygons.
- `Polygon.Area`, `Center` and `BoundingBox` were wrong for polygons that do not contain the origin.
- `LinearEquationSystem.Solve` validated the number of equations instead of coefficients.
- Expressions:
  - Unary minus replaced the first `-` in the whole string.
  - Unary `+` was ignored incorrectly.
  - Signs after operators (`2*-3`, `2^-1`) failed.
  - `%` and `log(x, base)` could not be used.
  - `.5` and `1e5` could not be parsed.
  - Unbalanced brackets were ignored or crashed.
  - Names with a common prefix (e.g. `sinh`) could not be added.
- Canvas (Fixes #3):
  - Zooming moved the origin by 1.25 times the distance to the cursor, so the point under the cursor drifted until
    coordinates overflowed.
  - Coordinates passed to GDI+ were not clipped.
  - The tick algorithm normalized large and small values incorrectly and threw for a height of 0.
  - A grid size of 0 made the drawing loops hang.
  - Function graphs were connected across poles.
  - `FunctionWrapper` cached every sampled value forever.
  - Pens and brushes were not disposed.
  - An origin of (0, 0) could not be set.

### Added

- `Tolerance` with `AreEqual`, `IsZero` and `IsNegligible`, and `ApproximatelyEquals` (with optional tolerances) on
  vectors, points, matrices, lines, segments, polygons and bounding boxes.
- `MatrixMxN`: an immutable matrix of any size with LU-based determinant, inverse, rank and `Solve`.
- `LineSegment2D`, `BoundingBox2D`, `IntersectionKind`, `LineIntersection2D`, `LineIntersection3D` and
  `SegmentIntersection2D`.
- `Line2D`:
  - factories `FromGeneralForm`, `FromSlopeIntercept`, `FromPoints`, `FromPointAndDirection`
  - `A`, `B`, `C`, `Normal`, `Direction`, `IsVertical`, `IsHorizontal`
  - `SolveForX`, `SignedDistanceTo`, `DistanceTo`, `ClosestPoint`, `PointAt`
- `Line3D`: `DistanceTo` (point and line), `ClosestPoint`, `ClosestPoints`, `IsParallelTo`, and `Intersect`
  including skew lines.
- Uniform operators on all vectors (`Vector4` had none): `+`, `-`, unary `-`, `v * s`, `s * v`, `v / s`.
- `Vector3(Vector2, z)`, `Vector4(Vector3, w)`, `Vector3.FromHomogeneous`, `ToPoint`, `Point2D/3D.ToVector`,
  `Point2D/3D.Distance` and `DistanceSquared` (instance and static).
- `Matrix4x4.Translation(Vector3)`, `Matrix2x2.Rotation/Scaling`, `Matrix3x3.Scaling(scale)`, and `ToString()` for
  all matrices.
- Expressions:
  - `MathExpression` with variables, `Evaluate(name, value)`, `Evaluate(dictionary)` and an allocation-free `ToFunc`
  - `ExpressionContext` for custom functions, constants and multi-letter variables, with `Clone()`
  - more built-ins: `atan2 sinh cosh tanh log2 exp floor ceil round sign pow min max tau`
- Canvas:
  - the platform-neutral `Canvas2D` model with `Viewport`, `CanvasStyle`, `ItemStyle` and the `Changed` event
  - items for points, infinite lines, segments, polygons and vectors with any tail
  - functions from expression strings, optionally with an `ExpressionContext`
  - the renderer interface `IDrawingContext`, with an SVG renderer (`RenderSvg`), a SkiaSharp renderer (`RenderPng`,
    `RenderImage`) and a GDI+ renderer
  - an Avalonia control (`Canvas2DView`) for Windows, Linux and macOS
- Samples for WinForms, Avalonia and headless export; BenchmarkDotNet benchmarks (`doc/benchmarks.md`); xUnit tests
  with property-based tests (CsCheck), zero-allocation tests and a coverage gate; GitHub Actions on Windows, Linux and
  macOS. Refs #5: the test suite now runs on all three platforms (see "Platform support and determinism" in the
  README).

### Performance

- Closed-form determinants, adjugates and inverses for 2x2 to 4x4 matrices instead of recursive Laplace expansion.
- Unrolled, non-boxing operators. Equality and hash codes no longer go through enumerators and LINQ.
- Common geometry and expression operations no longer allocate.

See [doc/benchmarks.md](doc/benchmarks.md) for measurements.

## [1.14.7] and earlier

Releases before 2.0 targeted .NET Framework 4.0 and are not documented in this file.

[2.0.0]: https://github.com/dbforge/SharpMath/compare/66339fd...refactor/v2
[1.14.7]: https://github.com/dbforge/SharpMath/tree/66339fd
