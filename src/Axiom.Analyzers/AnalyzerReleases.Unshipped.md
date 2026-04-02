### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1032 | Migration | Info | Suggest migrating MSTest Assert.AreEqual(expected, actual) to actual.Should().Be(expected).
AXM1033 | Migration | Info | Suggest migrating MSTest Assert.AreNotEqual(expected, actual) to actual.Should().NotBe(expected).
AXM1034 | Migration | Info | Suggest migrating MSTest Assert.IsNull(value) to value.Should().BeNull().
AXM1035 | Migration | Info | Suggest migrating MSTest Assert.IsNotNull(value) to value.Should().NotBeNull().
AXM1036 | Migration | Info | Suggest migrating MSTest Assert.IsTrue(condition) to condition.Should().BeTrue().
AXM1037 | Migration | Info | Suggest migrating MSTest Assert.IsFalse(condition) to condition.Should().BeFalse().
AXM1038 | Migration | Info | Suggest migrating MSTest Assert.AreSame(expected, actual) to actual.Should().BeSameAs(expected).
AXM1039 | Migration | Info | Suggest migrating MSTest Assert.AreNotSame(expected, actual) to actual.Should().NotBeSameAs(expected).
