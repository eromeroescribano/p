using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationQueue
{
    private Queue<Conversation> convesationsQueue=new Queue<Conversation>();
    public Conversation top() { return convesationsQueue.Peek();}

    public void Enqueue(Conversation con) { convesationsQueue.Enqueue(con); }
    public void EnqueuePriority(Conversation con) 
    { 
        Queue<Conversation>queue=new Queue<Conversation>();
        queue.Enqueue(con);
        while (convesationsQueue.Count > 0)
        {
                queue.Enqueue(convesationsQueue.Dequeue());
        }
        convesationsQueue = queue;
    }
    public void Dequeue() 
    {
        if (convesationsQueue.Count > 0)
        {
            convesationsQueue.Dequeue();
        }
    }
    public bool IsEmpty() { return convesationsQueue.Count == 0; }

    public void Clear() {  convesationsQueue.Clear(); }
}
