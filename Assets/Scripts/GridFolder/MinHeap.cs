using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> MinHeapList = new List<T>();

    // Build lại Min Heap khi xóa phần tử ở đầu
    private void HeaptifyDown(T parentNode, int index) // O(log n)
    {
        int smallest = index;
        int leftIndexChildren = index * 2 + 1;
        int rightIndexChildren = index * 2 + 2;

        if (leftIndexChildren < MinHeapList.Count && Compare(parentNode, getNode(leftIndexChildren)) >= 1)
            smallest = leftIndexChildren;

        if (rightIndexChildren < MinHeapList.Count && Compare(getNode(smallest), getNode(rightIndexChildren)) >= 1)
            smallest = rightIndexChildren;

        if (smallest != index)
        {
            Swap(smallest, index);
            HeaptifyDown(getNode(smallest), smallest);
        }
    }

    public void removeNodeAtFirst() // O(log n)
    {
        MinHeapList[0] = MinHeapList[MinHeapList.Count - 1];
        MinHeapList.RemoveAt(MinHeapList.Count - 1);
        HeaptifyDown(getNode(0), 0);
    }

    // Build lại Min Heap khi thêm phần tử vào cuối
    private void HeaptifyUp(T parentNode, int index) // index == MinHeapList.Count-1 && O(log n)
    {
        int idxParent = (index - 1) / 2;
        if (idxParent < 0)
            return;

        if (Compare(parentNode, getNode(index)) >= 1)
        {
            Swap(idxParent, index);
            HeaptifyUp(getNode((idxParent - 1) / 2), idxParent);
        }
    }

    public void addNewNode(T newNode) // O(log n)
    {
        if (MinHeapList == null)
        {
            return;
        }
        MinHeapList.Add(newNode);
        HeaptifyUp(getNode((MinHeapList.Count - 1 - 1) / 2), MinHeapList.Count - 1);
    }

    public T getLowestFCostNode() // O(1)
    {
        if (MinHeapList.Count <= 0)
        {
            return default(T);
        }
        return MinHeapList[0];
    }

    private int Compare(T item1, T item2)
    { // so sánh hai phương thức kiểu T chưa biết && O(1)
        return item1.CompareTo(item2); // tham chiếu tới hàm so sánh của class T của items
    }

    private T getNode(int index) // O(1)
    {
        if (index < MinHeapList.Count)
        {
            return MinHeapList[index];
        }
        return default(T);
    }

    private void Swap(int i, int j) // O(1)
    {
        if (i >= 0 && i < MinHeapList.Count && j >= 0 && j < MinHeapList.Count)
        {
            T tmpNode = getNode(i);
            MinHeapList[i] = MinHeapList[j];
            MinHeapList[j] = tmpNode;
        }
    }

    public int getCount()
    {
        return MinHeapList.Count;
    }
}