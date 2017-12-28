using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie2
{
    class ConcurrentQueue <T>{
		private class Node<E>{
            public readonly E Item;
            public volatile Node<E> Next;
            public Node(E item,Node<E> next){
                this.Item = item;
                this.Next = next;
            }
		}

        private volatile Node<T> head;
        private volatile Node<T> tail;

        public ConcurrentQueue(){
            Node<T> dummy = new Node<T>(default(T), null);
            this.head = dummy;
            this.tail = dummy;
        }

        public void Put(T item){
            Node<T> newNode = new Node<T>(item, null);
            Node<T> currTail;
            Node<T> tailNext;
            while(true){
                currTail = tail;
                tailNext = currTail.Next;
                if(currTail == tail){
                    if(tailNext != null){
                         Interlocked.CompareExchange(ref tail, tailNext, currTail);
                    }
                    else{
                        if(Interlocked.CompareExchange(ref currTail.Next,newNode,null) == null){
                            Interlocked.CompareExchange(ref tail, newNode, currTail);
                            return;
                        }
                    }
                }
            }
        }

        public T TryTake(){
            Node<T> headCurr;
            Node<T> tailCurr;
            Node<T> headNext;
            while(true){
                headCurr = head;
                tailCurr = tail;
                headNext = headCurr.Next;
                if(headCurr == head){
                    if(headCurr == tailCurr){
                        if(headNext == null){
                            return default(T);
                        }
                        Interlocked.CompareExchange(ref tail, headNext, tailCurr);
                    }
                    else{
                        if(Interlocked.CompareExchange(ref head, headNext, headCurr) == headCurr){
                            return headNext.Item;
                        }
                    }
                }
            }
        }

        public T Take(){
            T v;
            while((v = TryTake()) == null){
                Thread.Sleep(0);
            }
            return v;
        }

        public bool IsEmpty(){
            Node<T> currTail = tail;
            return Interlocked.CompareExchange(ref head, currTail, currTail) == currTail;
        }

	}
    

}
