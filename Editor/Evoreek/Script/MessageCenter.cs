using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageCenter {
    private MessageRegister register = new MessageRegister();

    public void Reg(int id, Action a) {
        register.Register(id, a);
    }

    public void Reg<T>(int id, Action<T> a) {
        register.Register<T>(id, a);
    }

    public void Reg<T1, T2>(int id, Action<T1, T2> a) {
        register.Register<T1, T2>(id, a);
    }

    public void Reg<T1, T2, T3>(int id, Action<T1, T2, T3> a) {
        register.Register<T1, T2, T3>(id, a);
    }
    
    public void Reg<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> a) {
        register.Register<T1, T2, T3, T4>(id, a);
    }
    
    public void Reg<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> a) {
        register.Register<T1, T2, T3, T4, T5>(id, a);
    }

    public void UnReg(int id, Action a) {
        register.UnRegister(id, a);
    }

    public void UnReg<T>(int id, Action<T> a) {
        register.UnRegister<T>(id, a);
    }

    public void UnReg<T1, T2>(int id, Action<T1, T2> a) {
        register.UnRegister<T1, T2>(id, a);
    }

    public void UnReg<T1, T2, T3>(int id, Action<T1, T2, T3> a) {
        register.UnRegister<T1, T2, T3>(id, a);
    }
    
    public void UnReg<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> a) {
        register.UnRegister<T1, T2, T3, T4>(id, a);
    }
    
    public void UnReg<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> a) {
        register.UnRegister<T1, T2, T3, T4, T5>(id, a);
    }


    public void Dispatcher(int id) {
        register.Dispatcher(id);
    }

    public void Dispatcher<T>(int id, T t) {
        register.Dispatcher<T>(id, t);
    }

    public void Dispatcher<T1, T2>(int id, T1 t1, T2 t2) {
        register.Dispatcher<T1, T2>(id, t1, t2);
    }

    public void Dispatcher<T1, T2, T3>(int id, T1 t1, T2 t2, T3 t3) {
        register.Dispatcher<T1, T2, T3>(id, t1, t2, t3);
    }

    public void Dispatcher<T1, T2, T3, T4>(int id, T1 t1, T2 t2, T3 t3, T4 t4) {
        register.Dispatcher<T1, T2, T3, T4>(id, t1, t2, t3, t4);
    }

    public void Dispatcher<T1, T2, T3, T4, T5>(int id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
        register.Dispatcher<T1, T2, T3, T4, T5>(id, t1, t2, t3, t4, t5);
    }

    public void Clear() {
        register.Clear();
    }
}

public class MessageRegister {
    private Dictionary<int, List<Act>> temps = new Dictionary<int, List<Act>>();

    public void Register(int id, Action e) {
        Register(id, (Delegate)e);
    }

    public void Register<T>(int id, Action<T> e) {
        Register(id, (Delegate)e);
    }

    public void Register<T1, T2>(int id, Action<T1, T2> e) {
        Register(id, (Delegate)e);
    }

    public void Register<T1, T2, T3>(int id, Action<T1, T2, T3> e) {
        Register(id, (Delegate)e);
    }
    
    public void Register<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> e) {
        Register(id, (Delegate)e);
    }
    
    public void Register<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> e) {
        Register(id, (Delegate)e);
    }

    private void Register(int id, Delegate e) {
        if (temps.TryGetValue(id, out var tmp)) {
            Act a = new Act() {
                handler = e
            };
            if (!tmp.Contains(a)) {
                tmp.Add(a);
                temps[id] = tmp;
            }
        } else {
            temps.Add(id, new List<Act>(){new Act() {
                handler = e
            }});
        }
    }

    public void UnRegister(int id, Action e) {
        UnRegister(id, (Delegate)e);
    }

    public void UnRegister<T>(int id, Action<T> e) {
        UnRegister(id, (Delegate)e);
    }

    public void UnRegister<T1, T2>(int id, Action<T1, T2> e) {
        UnRegister(id, (Delegate)e);
    }

    public void UnRegister<T1, T2, T3>(int id, Action<T1, T2, T3> e) {
        UnRegister(id, (Delegate)e);
    }
    
    public void UnRegister<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> e) {
        UnRegister(id, (Delegate)e);
    }
    
    public void UnRegister<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> e) {
        UnRegister(id, (Delegate)e);
    }

    private void UnRegister(int id, Delegate e) {
        if (temps.TryGetValue(id, out var tmp)) {
            Act a = new Act() {
                handler = e
            };
            if (tmp.Contains(a)) {
                tmp.Remove(a);
                temps[id] = tmp;
            }
        }
    }

    public void Dispatcher(int id) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var t in tmp) {
                t.Invoke();
            }
        }
    }

    public void Dispatcher<T>(int id, T t) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var i in tmp) {
                i.Invoke(t);
            }
        }
    }

    public void Dispatcher<T1, T2>(int id, T1 t1, T2 t2) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var i in tmp) {
                i.Invoke(t1, t2);
            }
        }
    }

    public void Dispatcher<T1, T2, T3>(int id, T1 t1, T2 t2, T3 t3) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var i in tmp) {
                i.Invoke(t1, t2, t3);
            }
        }
    }

    public void Dispatcher<T1, T2, T3, T4>(int id, T1 t1, T2 t2, T3 t3, T4 t4) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var i in tmp) {
                i.Invoke(t1, t2, t3, t4);
            }
        }
    }

    public void Dispatcher<T1, T2, T3, T4, T5>(int id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
        if (temps.TryGetValue(id, out var tmp)) {
            foreach (var i in tmp) {
                i.Invoke(t1, t2, t3, t4, t5);
            }
        }
    }

    public void Clear() {
        temps.Clear();
    }
}

public class Act {
    public Delegate handler;

    public void Invoke() {
        ((Action)handler).Invoke();
    }

    public void Invoke<T>(T t) {
        ((Action<T>)handler).Invoke(t);
    }

    public void Invoke<T1, T2>(T1 t1, T2 t2) {
        ((Action<T1, T2>)handler).Invoke(t1, t2);
    }

    public void Invoke<T1, T2, T3>(T1 t1, T2 t2, T3 t3) {
        ((Action<T1, T2, T3>)handler).Invoke(t1, t2, t3);
    }
    
    public void Invoke<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) {
        ((Action<T1, T2, T3, T4>)handler).Invoke(t1, t2, t3, t4);
    }
    
    public void Invoke<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
        ((Action<T1, T2, T3, T4, T5>)handler).Invoke(t1, t2, t3, t4, t5);
    }
    
}