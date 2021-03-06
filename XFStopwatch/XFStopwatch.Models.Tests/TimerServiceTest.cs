// <copyright file="TimerTest.cs">Copyright ©  2014</copyright>

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XFStopwatch.Models.Tests
{
    /// <summary>このクラスには、Timer に対するパラメーター化された単体テストが含まれています</summary>
    [TestClass]
    public class TimerServiceTest
    {
        /// <summary>
        /// IntervalがZeroの場合にエラーが発生することを確認する
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TimerTest_UninitializedInterval()
        {
            var timer = new TimerService();
            timer.Start();
        }

        /// <summary>
        /// 感覚を0.1秒単位に設定し、1.05秒実行した際に正しく動作していることを確認する
        /// </summary>
        [TestMethod]
        public void TimerTest_Normal()
        {
            var timer = new TimerService {Interval = TimeSpan.FromMilliseconds(100)};
            var beforeTime = DateTime.Now;
            var notifications = new List<TimeSpan>();
            timer.Elapsed += (sender, e) =>
            {
                // 開始前時間からの経過時間を保持しておく
                notifications.Add(DateTime.Now - beforeTime);
            };
            timer.Start();
            // タイマーを動作させるため、スレッドをスリープさせる
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            timer.Stop();

            // スレッドスイッチのオーバーヘッドがあるため、スリープ時間をIntervalで割った数分きっかり
            // 通知が来たりしない。
            // インターバル100[ms]で、待機時間が1000[ms]の為、5回以上通知が来ていれば良いものとする
            int count = notifications.Count;
            Assert.IsTrue(5 < notifications.Count);

            // Intervalの±20%の間に収まっていれば良いものとする
            for (int i = 0; i < notifications.Count - 1; i++)
            {
                Assert.IsTrue(TimeSpan.FromMilliseconds(100 * 0.8) < (notifications[i + 1] - notifications[i]));
                Assert.IsTrue((notifications[i + 1] - notifications[i]) < TimeSpan.FromMilliseconds(100 * 1.2));
            }

            // 本当に停止しているか確認する
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Assert.AreEqual(count, notifications.Count);
        }

        /// <summary>
        /// 一度終了後に再開できることを確認する
        /// </summary>
        [TestMethod]
        public void TimerTest_Resume()
        {
            var timer = new TimerService { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Start();
            timer.Stop();
            TimerTest_Normal();
        }

        /// <summary>
        /// 開始前に終了してもエラーが発生しないことを確認する
        /// </summary>
        [TestMethod]
        public void TimerTest_StopBeforeStart()
        {
            var timer = new TimerService();
            timer.Stop();
        }
    }
}
