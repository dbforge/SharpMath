# Benchmarks

Measured with [BenchmarkDotNet](https://benchmarkdotnet.org/) 0.15.8 on 2026-10-11:

```
Linux Ubuntu 22.04.5 LTS, 13th Gen Intel Core i7-13700H (laptop), 20 logical cores
.NET SDK 10.0.401, .NET 10.0.12, X64 RyuJIT x86-64-v3
```

**1.x** is the last 1.x commit (`66339fd`) compiled unchanged for `net10.0`, measured with equivalent benchmarks. The 1.x
sources are not part of the repository, so that column cannot be reproduced from this branch. **2.0** is this branch:

```shell
dotnet run -c Release --project benchmarks/SharpMath.Benchmarks -- --filter "*"
```

> **How to read the numbers.** The machine is a laptop with aggressive power management. Separate runs of the same
> benchmark varied by up to ±30 %, and one run that coincided with throttling was three times slower. The 10× to
> 1000× ratios below are robust. Where 1.x and 2.0 are close (polygon, parser), the numbers come from the default job,
> with 1.x and 2.0 run alternately twice; the averages are shown. The other rows come from `--job short` runs.
> Results below about 1 ns mean the JIT inlined the operation completely; they cannot be resolved more precisely.

## Matrices

| Benchmark | 1.x mean | 1.x allocated | 2.0 mean | 2.0 allocated |
|---|---:|---:|---:|---:|
| `Matrix2x2.Determinant` | 4.8 ns | 48 B | 0.3 ns | – |
| `Matrix3x3.Determinant` | 336 ns | 232 B | 0.3 ns | – |
| `Matrix4x4.Determinant` | 1,648 ns | 1,072 B | 2.7 ns | – |
| `Matrix2x2.Adjugate` | 111 ns | 288 B | 0.3 ns | – |
| `Matrix3x3.Adjugate` | 455 ns | 1,224 B | 1.6 ns | – |
| `Matrix4x4.Adjugate` | 5,344 ns | 6,016 B | 22 ns | – |
| `Matrix2x2.Inverse` ¹ | 140 ns | 384 B | 17 ns | – |
| `Matrix3x3.Inverse` ¹ | 493 ns | 1,584 B | 43 ns | – |
| `Matrix4x4.Inverse` ¹ | 946 ns | 3,744 B | 121 ns | – |
| `Matrix4x4.IsSingular` ² | 1,566 ns | 1,072 B | 87 ns | – |
| `Matrix3x3 * Matrix3x3` | 253 ns | 176 B | 4.1 ns | – |
| `Matrix4x4 * Matrix4x4` | 535 ns | 288 B | 23 ns | – |
| `Matrix4x4 * Vector3` | 177 ns | 288 B | 2.4 ns | – |
| `Matrix4x4 ==` | 42 ns | 608 B | 3.2 ns | – |
| `Matrix4x4.GetHashCode` | 47 ns | – | 40 ns | – |
| `Matrix4x4.IsOrthogonal` | 668 ns | 896 B | 57 ns | – |

¹ In 1.x, `Inverse` returned the identity matrix instead of the inverse (see the CHANGELOG), so those numbers measure
a wrong result. In 2.0, `Inverse` also checks whether the matrix is singular.

² 1.x compared the determinant with an absolute epsilon. 2.0 runs a pivoted LU decomposition, which also gives
correct results for badly scaled matrices.

## Vectors

| Benchmark | 1.x mean | 1.x allocated | 2.0 mean | 2.0 allocated |
|---|---:|---:|---:|---:|
| `a + b * 2` (`Vector3`) | 20 ns | – | 0.3 ns | – |
| `Vector3.Normalize` | 101 ns | – | 6.8 ns | – |
| `Vector3.Length` (`Magnitude`) | 31 ns | – | 0.3 ns | – |
| `Vector3 ==` | 24 ns | 192 B | 0.3 ns | – |
| `Vector3.IsZero` | 14 ns | 96 B | < 0.1 ns | – |
| `Vector3.GetHashCode` | 1.2 ns | – | 2.3 ns | – |
| `Vector3.IsParallelTo` (`CheckForParallelism`) ³ | 30 ns | 192 B | 18 ns | – |

³ 1.x divided the components and was wrong for vectors with zero components. 2.0 normalizes both vectors, which
makes the result independent of their lengths.

## Geometry and equations

| Benchmark | 1.x mean | 1.x allocated | 2.0 mean | 2.0 allocated |
|---|---:|---:|---:|---:|
| `Polygon.Contains` (64 vertices) | 416 ns | 3,152 B | 322 ns | – |
| `Polygon.Area` (64 vertices) | 84 ns | – | 89 ns | – |
| `LinearEquationSystem.Solve`, n = 3 | 174 ns | 360 B | 139 ns | 432 B |
| `LinearEquationSystem.Solve`, n = 10 | 1,916 ns | 1,200 B | 945 ns | 2,080 B |
| `LinearEquationSystem.Solve`, n = 50 | 183,208 ns | 21,040 B | 50,521 ns | 41,600 B |
| `MatrixMxN.Solve`, n = 50 | n/a | | 47,256 ns | 21,152 B |

In 2.0, `Polygon.Contains` also treats points on the border consistently as inside, and `Solve` uses partial
pivoting for numerical stability. `LinearEquationSystem.Solve` copies the coefficients into a matrix first, which
accounts for the extra allocation compared with `MatrixMxN.Solve`.

## Expressions

| Benchmark | 1.x mean | 1.x allocated | 2.0 mean | 2.0 allocated |
|---|---:|---:|---:|---:|
| Parse and evaluate `3*4^2+sin(1)/2-7*(2+cos(0))` | 2,013 ns | 3,272 B | 1,301 ns | 3,104 B |
| Evaluate a parsed expression with a variable | n/a ⁴ | | 51 ns | – |
| Call `ToFunc("x")` delegate | n/a ⁴ | | 51 ns | – |

⁴ 1.x had no variables and no way to evaluate a parsed expression again; plotting `f(x)` from a string meant parsing
it once per point.

## Canvas

| Benchmark | 2.0 mean | 2.0 allocated |
|---|---:|---:|
| `Canvas2D.Render` into a no-op renderer, 800×600, three functions (including `tan`), a vector and a polygon | 375 µs | 2.2 KB |

The remaining allocations are mostly the label strings and the clipped polygon. The 1.x canvas drew directly with
GDI+ inside a WinForms control and cannot be measured without a window. Its function sampling cached every sampled
value forever and allocated new GDI objects on every paint.
