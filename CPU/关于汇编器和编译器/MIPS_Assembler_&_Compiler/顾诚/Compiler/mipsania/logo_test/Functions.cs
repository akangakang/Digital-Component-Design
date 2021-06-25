using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Interpreter;

namespace LogoScriptIDE
{
    class FuncPrint
       : IFunction
    {
        public FuncPrint(ConsoleWindow cw)
        {
            m_cw = cw;
        }
        private delegate void Delegate(string s);
        public Varible invoke(GcMachine m, int numParams) //Note that the interface has changed
        {
            Delegate func = new Delegate(m_cw.Write);
            Varible v;
            string str = "";
            for (int i = 1; i <= numParams; ++i) //param have ID from 1 to numParams(possibly 0)
            {
                v = m.GetLocalByID(i); //How to access each param, do not specify ID less than 1 or greater than numParams
                if (v.type == VarType.TYPE_NUMBER)
                {
                    str += v.num.ToString();
                }
                else if (v.type == VarType.TYPE_STRING)
                {
                    str += v.str;
                }
                else
                    throw new RuntimeException("Type mismatch"); //You can throw any runtime exception too.
            }
            str += "\n";
            m_cw.Dispatcher.Invoke(
                        func,
                        System.Windows.Threading.DispatcherPriority.ContextIdle,
                        str);
            return new Varible(0.0); //You can return any number here.
        }
        private ConsoleWindow m_cw;
    }

    //class FuncFD
    //    : IFunction
    //{
    //    public FuncFD(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
    //            throw new RuntimeException("Invaild arugment");

    //        m_turtle.MoveForward(m.GetLocalByID(1).num);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncBK
    //    : IFunction
    //{
    //    public FuncBK(Turtle t)
    //    {
    //        m_turtle = t;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
    //            throw new RuntimeException("Invaild arugment");

    //        m_turtle.MoveForward(-m.GetLocalByID(1).num);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncLT
    //    : IFunction
    //{
    //    public FuncLT(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
    //            throw new RuntimeException("Invaild arugment");

    //        m_turtle.RotateClockwise(-m.GetLocalByID(1).num);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncRT
    //    : IFunction
    //{
    //    public FuncRT(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
    //            throw new RuntimeException("Invaild arugment");

    //        m_turtle.RotateClockwise(m.GetLocalByID(1).num);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncPU
    //    : IFunction
    //{
    //    public FuncPU(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        m_turtle.PenUp = true;
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncPD
    //    : IFunction
    //{
    //    public FuncPD(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        m_turtle.PenUp = false;
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncRESET
    //    : IFunction
    //{
    //    public FuncRESET(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    private delegate void Delegate();
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        Delegate func = new Delegate(m_turtle.ResetAll);
    //        m_turtle.Dispatcher.Invoke(
    //            func,
    //            System.Windows.Threading.DispatcherPriority.ContextIdle);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncRESETA
    //    : IFunction
    //{
    //    public FuncRESETA(Turtle t)
    //    {
    //        m_turtle = t;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        m_turtle.ResetAngle();
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncSETSIZE
    //    : IFunction
    //{
    //    public FuncSETSIZE(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    private delegate void Delegate(double w, double h);
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 2 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(2).type != VarType.TYPE_NUMBER)
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }
    //        Delegate func = new Delegate(m_turtle.ResetCanvas);
    //        m_turtle.Dispatcher.Invoke(
    //            func,
    //            System.Windows.Threading.DispatcherPriority.ContextIdle,
    //            m.GetLocalByID(1).num, m.GetLocalByID(2).num);
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncSETXY
    //    : IFunction
    //{
    //    public FuncSETXY(Turtle t)
    //    {
    //        m_turtle = t;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 2 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(2).type != VarType.TYPE_NUMBER)
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }
    //        else
    //        {
    //            double x = m.GetLocalByID(1).num;
    //            double y = m.GetLocalByID(2).num;
    //            m_turtle.MoveTo(new Point(x, y));
    //        }
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncDELAY
    //    : IFunction
    //{
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 1 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(1).num < 0
    //            )
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }
    //        System.Threading.Thread.Sleep(Convert.ToInt32(m.GetLocalByID(1).num));
    //        return new Varible(0.0);
    //    }
    //}

    //class FuncSETPC
    //: IFunction
    //{
    //    public FuncSETPC(Turtle t)
    //    {
    //        m_turtle = t;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        if (numParams < 3 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(2).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(3).type != VarType.TYPE_NUMBER)
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }

    //        if ((m.GetLocalByID(1).num < 0) || (m.GetLocalByID(1).num > 255) ||
    //             (m.GetLocalByID(2).num < 0) || (m.GetLocalByID(2).num > 255) ||
    //             (m.GetLocalByID(3).num < 0) || (m.GetLocalByID(1).num > 255))
    //            throw new RuntimeException("Illegal arugment");

    //        m_turtle.SetPenColor(
    //            Convert.ToByte(m.GetLocalByID(1).num),
    //            Convert.ToByte(m.GetLocalByID(2).num),
    //            Convert.ToByte(m.GetLocalByID(3).num));

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncSETPW
    //: IFunction
    //{
    //    public FuncSETPW(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {

    //        if (numParams < 1 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER ||
    //            m.GetLocalByID(1).num <= 0)
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }

    //        m_turtle.SetWidth(m.GetLocalByID(1).num);

    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncSHOW
    //: IFunction
    //{
    //    public FuncSHOW(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        m_turtle.Show = true;
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncHIDE
    //: IFunction
    //{
    //    public FuncHIDE(Turtle t)
    //    {
    //        m_turtle = t;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        m_turtle.Show = false;
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncINPUT
    //: IFunction
    //{
    //    public FuncINPUT(Turtle t, MainWindow parent)
    //    {
    //        m_turtle = t;
    //        m_inputboxWindow = new InputboxWindow();
    //        m_inputboxWindow.Owner = parent;
    //    }
    //    private delegate void Delegate(string s);
    //    private double Ans;
    //    private bool isLegal;
    //    private InputboxWindow m_inputboxWindow;

    //    private void inputNum(string hint)
    //    {
    //        isLegal = true;
    //        if (hint != "") m_inputboxWindow.ui_showText.Text = hint;
    //        else m_inputboxWindow.ui_showText.Text = "Please input a double:";
    //        m_inputboxWindow.ShowDialog();
    //        if (m_inputboxWindow.inputStatus() == 1)
    //            Ans = m_inputboxWindow.getNum();
    //        else if (m_inputboxWindow.inputStatus() == -1)
    //            isLegal = false;
    //        else Ans = 0;
    //    }

    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        Delegate func = new Delegate(inputNum);
    //        Varible v;
    //        string str = "";
    //        for (int i = 1; i <= numParams; ++i) //param have ID from 1 to numParams(possibly 0)
    //        {
    //            v = m.GetLocalByID(i); //How to access each param, do not specify ID less than 1 or greater than numParams
    //            if (v.type == VarType.TYPE_NUMBER)
    //            {
    //                str += v.num.ToString();
    //            }
    //            else if (v.type == VarType.TYPE_STRING)
    //            {
    //                str += v.str;
    //            }
    //            else
    //                throw new RuntimeException("Type mismatch"); //You can throw any runtime exception too.
    //        }

    //        m_turtle.Dispatcher.Invoke(
    //            func,
    //            System.Windows.Threading.DispatcherPriority.ContextIdle,
    //            str);
    //        if (isLegal)
    //            return new Varible(Ans);
    //        else
    //            throw new RuntimeException("Type mismatch");
    //    }
    //    private Turtle m_turtle;
    //}

    //class FuncUPDATE
    //: IFunction
    //{
    //    public FuncUPDATE(Turtle t)
    //    {
    //        m_turtle = t;
    //    }
    //    public Varible invoke(GcMachine m, int numParams)
    //    {
    //        Turtle.UpdateMode mode;
    //        if (numParams < 1 ||
    //            m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
    //        {
    //            throw new RuntimeException("Invaild arugment");
    //        }

    //        switch (Convert.ToInt32(m.GetLocalByID(1).num))
    //        {
    //            case 1:
    //                mode = Turtle.UpdateMode.UPDATE_AUTO;
    //                break;
    //            case 2:
    //                mode = Turtle.UpdateMode.UPDATE_MANUAL;
    //                break;
    //            case 3:
    //                mode = Turtle.UpdateMode.UPDATE_NOW;
    //                break;
    //            default:
    //                throw new RuntimeException("Invaild arugment");
    //        }
    //        m_turtle.Update(mode);
    //        return new Varible(0.0);
    //    }
    //    private Turtle m_turtle;
    //}
}
