### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1040 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.Contain(expectedSubstring)) to actual.Should().Contain(expectedSubstring).
AXM1041 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.Not.Contain(expectedSubstring)) to actual.Should().NotContain(expectedSubstring).
AXM1042 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.StartWith(expectedPrefix)) to actual.Should().StartWith(expectedPrefix).
AXM1043 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.EndWith(expectedSuffix)) to actual.Should().EndWith(expectedSuffix).
AXM1044 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.Count.EqualTo(expectedCount)) to collection.Should().HaveCount(expectedCount).
AXM1045 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.SameAs(expected)) to actual.Should().BeSameAs(expected).
AXM1046 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.SameAs(expected)) to actual.Should().NotBeSameAs(expected).
