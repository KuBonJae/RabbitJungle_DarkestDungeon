using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PriorityQueue<T> where T : IComparable<T>
{
    T[] data;
    public int Count { get; private set; }
    public int Capacity { get; private set; }
    #region ������
    // �⺻ ������
    public PriorityQueue()
    {
        Count = 0;
        Capacity = 1;
        data = new T[Capacity];
    }
    // �ʱ� Capacity ���� ������
    public PriorityQueue(int capacity)
    {
        Count = 0;
        Capacity = capacity;
        data = new T[Capacity];
    }
    #endregion

    #region public function
    public void Enqueue(T value)
    {
        // data �迭�� �� á�ٸ� Ȯ��
        if (Count >= Capacity)
            Expand();
        // ������ �߰�
        data[Count] = value;
        Count++;

        // �� Ʈ���� �����ϱ� ���� ������ ��ȯ
        // ���� �߰��� ������ �θ� ���� ���Ͽ� �� ũ�ٸ� 
        int now = Count - 1;
        while (now > 0)
        {
            int parent = (now - 1) / 2;
            // �θ� ����� ���� �� ũ�ٸ� ����
            if (data[now].CompareTo(data[parent]) < 0)
                break;

            // �� ��ȯ
            T temp = data[now];
            data[now] = data[parent];
            data[parent] = temp;
            // ���� ��ġ ����
            now = parent;
        }

    }

    public T Dequeue()
    {
        // Count�� 0�̶�� ���� �߻�
        if (Count == 0)
            throw new IndexOutOfRangeException();

        // ��Ʈ ��� �� ����
        // ������ ���� ��ȯ �� ����
        T result = data[0];
        data[0] = data[Count - 1];
        data[Count - 1] = default(T);
        Count--;

        // �� Ʈ���� �����ϵ��� ������ ��ȯ
        // ��Ʈ���� �����Ͽ� �ڽ� ��� �� ū �ʰ� ��, ���� ��尡 �� �۴ٸ� ��ȯ
        int now = 0;
        while (now < Count)
        {
            int left = (now * 2) + 1;
            int right = (now * 2) + 2;

            int next = now;
            // ���� ��尡 �����ϰ� ���� �� ũ�ٸ� next ���� 
            if (left < Count && data[next].CompareTo(data[left]) < 0)
                next = left;
            // ������ ��尡 �����ϰ� ���� �� ũ�ٸ� next ���� 
            if (right < Count && data[next].CompareTo(data[right]) < 0)
                next = right;
            // ���ŵ��� �ʾҴٸ� ���� ����
            if (next == now)
                break;

            // �� ��ȯ
            T temp = data[now];
            data[now] = data[next];
            data[next] = temp;
            // ���� ��ġ ����
            now = next;
        }

        return result;
    }

    public T Peek()
    {
        // Count�� 0�̶�� ���� �߻�
        if (Count == 0)
            throw new IndexOutOfRangeException();

        return data[0];
    }
    #endregion

    #region private function
    // data �迭 Ȯ���
    // ���� Capacity�� 2��� Ȯ��
    void Expand()
    {
        T[] newData = new T[Capacity * 2];
        for (int i = 0; i < Count; i++)
            newData[i] = data[i];

        data = newData;
        Capacity *= 2;
    }
    #endregion
}

class PriorityQueue
{
    Tuple<int, int>[] data;
    public int Count { get; private set; }
    public int Capacity { get; private set; }
    #region ������
    // �⺻ ������
    public PriorityQueue()
    {
        Count = 0;
        Capacity = 1;
        data = new Tuple<int, int>[Capacity];
    }
    // �ʱ� Capacity ���� ������
    public PriorityQueue(int capacity)
    {
        Count = 0;
        Capacity = capacity;
        data = new Tuple<int, int>[Capacity];
    }
    #endregion

    #region public function
    public void Enqueue(Tuple<int, int> value)
    {
        // data �迭�� �� á�ٸ� Ȯ��
        if (Count >= Capacity)
            Expand();
        // ������ �߰�
        data[Count] = value;
        Count++;

        // �� Ʈ���� �����ϱ� ���� ������ ��ȯ
        // ���� �߰��� ������ �θ� ���� ���Ͽ� �� ũ�ٸ� 
        int now = Count - 1;
        while (now > 0)
        {
            int parent = (now - 1) / 2;
            // �θ� ����� ���� �� ũ�ٸ� ����
            if (data[now].Item2.CompareTo(data[parent].Item2) < 0)
                break;

            // �� ��ȯ
            Tuple<int, int> temp = data[now];
            data[now] = data[parent];
            data[parent] = temp;
            // ���� ��ġ ����
            now = parent;
        }

    }

    public Tuple<int, int> Dequeue()
    {
        // Count�� 0�̶�� ���� �߻�
        if (Count == 0)
            throw new IndexOutOfRangeException();

        // ��Ʈ ��� �� ����
        // ������ ���� ��ȯ �� ����
        Tuple<int, int> result = data[0];
        data[0] = data[Count - 1];
        data[Count - 1] = default(Tuple<int, int>);
        Count--;

        // �� Ʈ���� �����ϵ��� ������ ��ȯ
        // ��Ʈ���� �����Ͽ� �ڽ� ��� �� ū �ʰ� ��, ���� ��尡 �� �۴ٸ� ��ȯ
        int now = 0;
        while (now < Count)
        {
            int left = (now * 2) + 1;
            int right = (now * 2) + 2;

            int next = now;
            // ���� ��尡 �����ϰ� ���� �� ũ�ٸ� next ���� 
            if (left < Count && data[next].Item2.CompareTo(data[left].Item2) < 0)
                next = left;
            // ������ ��尡 �����ϰ� ���� �� ũ�ٸ� next ���� 
            if (right < Count && data[next].Item2.CompareTo(data[right].Item2) < 0)
                next = right;
            // ���ŵ��� �ʾҴٸ� ���� ����
            if (next == now)
                break;

            // �� ��ȯ
            Tuple<int, int> temp = data[now];
            data[now] = data[next];
            data[next] = temp;
            // ���� ��ġ ����
            now = next;
        }

        return result;
    }

    public Tuple<int, int> Peek()
    {
        // Count�� 0�̶�� ���� �߻�
        if (Count == 0)
            throw new IndexOutOfRangeException();

        return data[0];
    }
    #endregion

    #region private function
    // data �迭 Ȯ���
    // ���� Capacity�� 2��� Ȯ��
    void Expand()
    {
        Tuple<int, int>[] newData = new Tuple<int, int>[Capacity * 2];
        for (int i = 0; i < Count; i++)
            newData[i] = data[i];

        data = newData;
        Capacity *= 2;
    }
    #endregion
}