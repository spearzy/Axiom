### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1054 | Migration | Info | Suggest migrating awaited xUnit Assert.ThrowsAsync<TException>(...) to an Axiom awaitable .Should().ThrowExactlyAsync<TException>() assertion, chaining .WithParamName(...) for non-null constant param-name overloads and appending .Thrown when the exception is used.
AXM1055 | Migration | Info | Suggest migrating awaited xUnit Assert.ThrowsAnyAsync<TException>(...) to an Axiom awaitable .Should().ThrowAsync<TException>() assertion, appending .Thrown when the exception is used.
