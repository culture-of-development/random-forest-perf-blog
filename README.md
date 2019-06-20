# random-forest-perf-blog
The code to accompany the blog posts in my series on making random forests fast.

---

If you would like to follow along, please clone this repo.  To run any tests you must first have the [dotnet core sdk](https://dotnet.microsoft.com/download) installed, then browse to the `src` folder and choose one of the commands to run from below.

To run the naive tests:

```
dotnet test -c Release --filter "DisplayName~NaiveTests"
```
