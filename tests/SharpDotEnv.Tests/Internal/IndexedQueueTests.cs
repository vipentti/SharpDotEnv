using FluentAssertions;
using SharpDotEnv.Internal;
using System;
using System.Linq;
using Xunit;

namespace SharpDotEnv.Tests.Internal
{
    public class IndexedQueueTests
    {
        private readonly IndexedQueue<int> collection = new IndexedQueue<int>();

        [Fact]
        public void CountIsInitiallyZero()
        {
            collection.Should().HaveCount(0);
        }

        [Fact]
        public void Supports_Enqueue()
        {
            collection.Enqueue(0);
            collection.Should().HaveCount(1);
            collection[0].Should().Be(0);
        }

        [Fact]
        public void MaintainsOrderWhenEnqueuingMultipleItems()
        {
            var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            foreach (var item in itemsToAdd)
            {
                collection.Enqueue(item);
            }

            collection.Should().HaveCount(itemsToAdd.Length);
            collection.Should().BeEquivalentTo(itemsToAdd);
        }

        [Fact]
        public void CanBeEnumerated()
        {
            var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            foreach (var item in itemsToAdd)
            {
                collection.Enqueue(item);
            }

            collection.AsEnumerable().ToArray().Should().BeEquivalentTo(itemsToAdd);
        }

        [Fact]
        public void ClearClearsTheContents()
        {
            var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            foreach (var item in itemsToAdd)
            {
                collection.Enqueue(item);
            }

            collection.Should().HaveCount(itemsToAdd.Length);

            collection.Clear();

            collection.Should().HaveCount(0);
        }


        public class LastIndexOf
        {
            private readonly IndexedQueue<int> collection = new IndexedQueue<int>();

            [Fact]
            public void ReturnsNegativeOneWhenEmpty()
            {
                collection.LastIndexOf(_ => true).Should().Be(-1);
                collection.LastIndexOf(0).Should().Be(-1);
            }

            [Fact]
            public void ReturnsNegativeOneWhenNotFound()
            {
                var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                foreach (var item in itemsToAdd)
                {
                    collection.Enqueue(item);
                }

                collection.LastIndexOf(8).Should().Be(-1);
            }

            [Fact]
            public void ReturnsLastIndexOfSingleItems()
            {
                var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                foreach (var item in itemsToAdd)
                {
                    collection.Enqueue(item);
                }

                collection.LastIndexOf(7).Should().Be(7);
                collection.LastIndexOf(6).Should().Be(6);
                collection.LastIndexOf(4).Should().Be(4);
            }

            [Fact]
            public void ReturnsAfterMultipleEnqueuesAndDequeues()
            {
                var itemsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                foreach (var item in itemsToAdd)
                {
                    collection.Enqueue(item);
                }

                collection.Dequeue().Should().Be(0);
                collection.Dequeue().Should().Be(1);
                collection.Dequeue().Should().Be(2);

                collection.Enqueue(11);
                collection.Dequeue().Should().Be(3);
                collection.Enqueue(12);

                collection.LastIndexOf(4).Should().Be(0);
                collection.LastIndexOf(7).Should().Be(3);
                collection[0].Should().Be(4);
                collection[3].Should().Be(7);
            }
        }

        public class Dequeue
        {
            private readonly IndexedQueue<int> collection = new IndexedQueue<int>();

            [Fact]
            public void ThrowsWhenCollectionIsEmpty()
            {
                Func<int> act = () => collection.Dequeue();
                act.Should().ThrowExactly<IndexOutOfRangeException>()
                    .WithMessage("IndexedQueue is empty.");
            }

            [Fact]
            public void TryDequeueReturnsFalseWhenCollectionIsEmpty()
            {
                collection.TryDequeue(out _).Should().BeFalse();
            }

            [Fact]
            public void RemovesItemsInCorrectOrder()
            {
                collection.Enqueue(0);
                collection.Enqueue(1);

                collection.Dequeue().Should().Be(0);
                collection.Dequeue().Should().Be(1);
                collection.Should().BeEmpty();
            }
        }

        public class Peek
        {
            private readonly IndexedQueue<int> collection = new IndexedQueue<int>();

            [Fact]
            public void ThrowsWhenCollectionIsEmpty()
            {
                Func<int> act = () => collection.Peek();
                act.Should().ThrowExactly<IndexOutOfRangeException>()
                    .WithMessage("IndexedQueue is empty.");
            }

            [Fact]
            public void TryPeekReturnsFalseWhenCollectionIsEmpty()
            {
                collection.TryPeek(out _).Should().BeFalse();
            }

            [Fact]
            public void DoesNotRemoveElements()
            {
                collection.Enqueue(0);
                collection.Enqueue(1);

                collection.Peek().Should().Be(0);
                collection.Should().HaveCount(2);
            }

            [Fact]
            public void PeeksItemsInCorrectOrder()
            {
                collection.Enqueue(0);
                collection.Enqueue(1);

                collection.Peek().Should().Be(0);
            }

            [Fact]
            public void TryPeekPeeksItemsInCorrectOrder()
            {
                collection.Enqueue(0);
                collection.Enqueue(1);

                collection.TryPeek(out var item0).Should().BeTrue();
                item0.Should().Be(0);
            }
        }
    }
}
