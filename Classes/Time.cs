using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Classes
{
    public class Time
    {
        #region Public constants

        public const char TIME_SEPERATOR = ':';

        #endregion

        #region Declarations

        public int Hour;
        public int Minute;
        public int Second;

        #endregion

        #region Constructors

        /// <summary>
        /// Cria um object com o tempo corrente.
        /// </summary>
        public Time()
        {
            Hour = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;
            Second = DateTime.Now.Second;
        }

        /// <summary>
        /// cria um object a partir de uma string com seperatedo TIME_SEPERATOR.
        /// </summary>
        /// <param name="value"></param>
        public Time(string value)
        {
            string[] vals = value.Split(TIME_SEPERATOR); //new char[] { ':' });
            Hour = int.Parse(vals[0]);
            Minute = int.Parse(vals[1]);

            if (vals.Length > 2)
                Second = int.Parse(vals[2]);

            new Time(this.ToSeconds());
        }

        /// <summary>
        /// Cria um objeto time a partir data hora, minuto e segundo.
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public Time(int hour, int minute, int second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
            new Time(this.ToSeconds());
        }

        /// <summary>
        /// cria um objeto time a partir dos segundos.
        /// </summary>
        /// <param name="seconds"></param>
        public Time(int seconds)
        {
            Minute = seconds / 60;
            Second = seconds % 60;

            Hour = Minute / 60;
            Minute = Minute % 60;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// soma um objeto time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Time Add(Time time)
        {
            this.Hour += time.Hour;
            this.Minute += time.Minute;
            this.Second += time.Second;

            return new Time(GetStringTime(this.ToSeconds()));
        }

        /// <summary>
        /// Soma um objto time passando a hora como string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Time Add(string value)
        {
            return Add(new Time(value));
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Retorna o tempo corrente.
        /// </summary>
        /// <returns></returns>
        public static Time Now()
        {
            DateTime dt = DateTime.Now;
            return GetTimeFromSeconds(ToSeconds(dt));
        }

        /// <summary>
        /// Calcula a diferença entre dois times.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static Time TimeDiff(Time time1, Time time2)
        {
            try
            {
                int _secs1 = time1.ToSeconds();
                int _secs2 = time2.ToSeconds();

                int _secs = _secs1 - _secs2;

                return GetTimeFromSeconds(_secs);
            }
            catch
            {
                return new Time(0, 0, 0);
            }

        }

        /// <summary>
        /// Calcula a diferença de dois time entre duas string.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static Time TimeDiff(string time1, string time2)
        {
            try
            {
                Time t1 = new Time(time1);
                Time t2 = new Time(time2);
                return TimeDiff(t1, t2);
            }
            catch
            {
                return new Time(0, 0, 0);
            }
        }

        /// <summary>
        /// Calcula a diferença de dois time entre dois objetos DateTime.
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static Time TimeDiff(DateTime dateTime1, DateTime dateTime2)
        {
            try
            {
                TimeSpan span = dateTime1 - dateTime2;
                return new Time(span.Hours, span.Minutes, span.Seconds);
            }
            catch
            {
                return new Time(0, 0, 0);
            }
        }

        /// <summary>
        /// Calcula a diferença de dois time entre dois objetos inteiros (segundos).
        /// </summary>
        /// <param name="seconds1"></param>
        /// <param name="seconds2"></param>
        /// <returns></returns>
        public static Time TimeDiff(int seconds1, int seconds2)
        {
            try
            {
                Time t1 = new Time(seconds1);
                Time t2 = new Time(seconds2);
                return TimeDiff(t1, t2);
            }
            catch
            {
                return new Time(0, 0, 0);
            }
        }

        #endregion

        #region Convert methods

        /// <summary>
        /// Converte para segundos.
        /// </summary>
        /// <returns></returns>
        public int ToSeconds()
        {
            return this.Hour * 3600 + this.Minute * 60 + this.Second;
        }

        /// <summary>
        /// Converte para segundos a partir do DateTime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToSeconds(DateTime dateTime)
        {
            return dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second;
        }

        /// <summary>
        /// Coverte para string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0:00}:{1:00}:{2:00}", Hour, Minute, Second);
        }

        /// <summary>
        /// Converte segundos para Time.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Time GetTimeFromSeconds(int seconds)
        {
            int _mins = seconds / 60;
            seconds = seconds % 60;

            int _hours = _mins / 60;
            _mins = _mins % 60;

            return new Time(_hours, _mins, seconds);
        }


        /// <summary>
        /// Converte segundos para time em string.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private string GetStringTime(int seconds)
        {
            int _mins = seconds / 60;
            seconds = seconds % 60;

            int _hours = _mins / 60;
            _mins = _mins % 60;

            this.Hour = _hours;
            this.Minute = _mins;
            this.Second = seconds;

            return String.Format("{0:00}:{1:00}:{2:00}", _hours, _mins, seconds); ;
        }

        /// <summary>
        /// Paser string para time.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Time Parse(string value)
        {
            try
            {
                return new Time(value);
            }
            catch
            {
                throw new ApplicationException("Error parsing time!");
            }
        }

        #endregion

        #region Subtract time objects

        public static Time operator +(Time t1, Time t2)
        {
            Time t3 = new Time(t1.Hour, t1.Minute, t1.Second);
            t3.Add(t2);
            return t3;
        }

        public static Time operator -(Time t1, Time t2)
        {
            return TimeDiff(t1, t2);
        }

        #endregion
    }
}


