## Release 0.7.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM0001 | Usage | Warning | Detect ignored async Axiom assertion calls.
AXM0002 | Usage | Warning | Detect Batch instances that are created without being disposed.

## Release 0.10.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1001 | Migration | Info | Suggest migrating xUnit Assert.Equal(expected, actual) to actual.Should().Be(expected).
AXM1002 | Migration | Info | Suggest migrating xUnit Assert.NotEqual(expected, actual) to actual.Should().NotBe(expected).
AXM1003 | Migration | Info | Suggest migrating xUnit Assert.Null(value) to value.Should().BeNull().
AXM1004 | Migration | Info | Suggest migrating xUnit Assert.NotNull(value) to value.Should().NotBeNull().
AXM1005 | Migration | Info | Suggest migrating xUnit Assert.True(condition) to condition.Should().BeTrue().
AXM1006 | Migration | Info | Suggest migrating xUnit Assert.False(condition) to condition.Should().BeFalse().
AXM1007 | Migration | Info | Suggest migrating xUnit Assert.Empty(subject) to subject.Should().BeEmpty().
AXM1008 | Migration | Info | Suggest migrating xUnit Assert.NotEmpty(subject) to subject.Should().NotBeEmpty().
AXM1009 | Migration | Info | Suggest migrating xUnit Assert.Contains(item, collection) to collection.Should().Contain(item).
AXM1010 | Migration | Info | Suggest migrating xUnit Assert.DoesNotContain(item, collection) to collection.Should().NotContain(item).
AXM1011 | Migration | Info | Suggest migrating xUnit Assert.Single(subject) to subject.Should().ContainSingle().
AXM1012 | Migration | Info | Suggest migrating xUnit Assert.Same(expected, actual) to actual.Should().BeSameAs(expected).
AXM1013 | Migration | Info | Suggest migrating xUnit Assert.NotSame(expected, actual) to actual.Should().NotBeSameAs(expected).
AXM1014 | Migration | Info | Suggest migrating xUnit Assert.Throws<TException>(...) to an Axiom .Should().Throw<TException>() assertion when the result is not consumed.
