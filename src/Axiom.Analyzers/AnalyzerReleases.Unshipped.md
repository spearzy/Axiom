### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1024 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.EqualTo(expected)) to actual.Should().Be(expected).
AXM1025 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.EqualTo(expected)) to actual.Should().NotBe(expected).
AXM1026 | Migration | Info | Suggest migrating NUnit Assert.That(value, Is.Null) to value.Should().BeNull().
AXM1027 | Migration | Info | Suggest migrating NUnit Assert.That(value, Is.Not.Null) to value.Should().NotBeNull().
AXM1028 | Migration | Info | Suggest migrating NUnit Assert.That(condition, Is.True) to condition.Should().BeTrue().
AXM1029 | Migration | Info | Suggest migrating NUnit Assert.That(condition, Is.False) to condition.Should().BeFalse().
AXM1030 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Empty) to collection.Should().BeEmpty().
AXM1031 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Not.Empty) to collection.Should().NotBeEmpty().
