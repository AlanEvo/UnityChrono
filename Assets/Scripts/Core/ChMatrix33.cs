﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace chrono
{


    ///
    /// ChMatrix33
    ///
    /// A special type of NxM matrix: the 3x3 matrix that is commonly used
    /// to represent coordinate transformations in 3D space.
    /// This matrix cannot be resized.
    /// The 3x3 matrix can be multiplied/added with other matrix types.
    ///
    /// Further info at the @ref manual_ChMatrix33 manual page.
    public struct ChMatrix33<Real>// : ChMatrixNM<IntInterface.Three, IntInterface.Three>
    {
        public ChMatrixNM<IntInterface.Three, IntInterface.Three> nm;
       // public static ChMatrix33<double> MNULL = new ChMatrix33<double>(0);

        //
        // CONSTRUCTORS
        //

        /// Default constructor builds a 3x3 matrix with zeroes.
       /* public ChMatrix33() {
        }*/

        /// Copy constructor
        public ChMatrix33(ChMatrix33<Real> msource) {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
            nm.matrix = new ChMatrix();
        }

        /// Copy constructor from all types of base matrices (only with same size)
        public ChMatrix33(ChMatrix msource) //: this()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
            //nm.matrix = new ChMatrix();
            // Debug.Assert(msource.GetColumns() == 3 && msource.GetRows() == 3);
            this.nm.matrix.rows = 3;
            this.nm.matrix.columns = 3;
             this.nm.matrix.address = this.nm.buffer;  
           /* this.address = (double*)Marshal.AllocHGlobal(this.buffer.Length * sizeof(double));
            for (int i = 0; i < this.buffer.Length; i++)
            {
                this.address[i] = this.buffer[i];
            }*/
            // ElementsCopy(this.address, msource.GetAddress(), 9);
            for (int i = 0; i < 9; ++i)
                this.nm.matrix.address[i] = msource.GetAddress()[i];
        }

        /// Construct a diagonal 3x3 matrix with all diagonal elements equal to the specified value.
        public ChMatrix33(double val) //: this()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
          //  nm.matrix = new ChMatrix();
            this.nm.matrix.Set33Element(0, 0, val);
            this.nm.matrix.Set33Element(1, 1, val);
            this.nm.matrix.Set33Element(2, 2, val);
        }

        public ChMatrix33(ChVector vec)// : this()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
           // nm.matrix = new ChMatrix();
            this.nm.matrix.Set33Element(0, 0, vec.x);
            this.nm.matrix.Set33Element(1, 1, vec.y);
            this.nm.matrix.Set33Element(2, 2, vec.z);
        }

        /// Construct a symmetric 3x3 matrix with the specified vectors for the
        /// diagonal and off-diagonal.  The off-diagonal vector is assumed to contain
        /// the elements A(0,1), A(0,2), A(1,2) in this order.
        public ChMatrix33(ChVector diag, ChVector off_diag) //: this()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
           // nm.matrix = new ChMatrix();
            this.nm.matrix.Set33Element(0, 0, diag.x);
            this.nm.matrix.Set33Element(1, 1, diag.y);
            this.nm.matrix.Set33Element(2, 2, diag.z);

            this.nm.matrix.Set33Element(0, 1, off_diag.x);
            this.nm.matrix.Set33Element(1, 0, off_diag.x);

            this.nm.matrix.Set33Element(0, 2, off_diag.y);
            this.nm.matrix.Set33Element(2, 0, off_diag.y);

            this.nm.matrix.Set33Element(1, 2, off_diag.z);
            this.nm.matrix.Set33Element(2, 1, off_diag.z);
        }

        /// The constructor which builds a 3x3 matrix given a quaternion representing rotation.
        public ChMatrix33(ChQuaternion mq) //: this()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
           // nm.matrix = new ChMatrix();
            this.nm.matrix.rows = 3;
            this.nm.matrix.columns = 3;
            this.nm.matrix.address = this.nm.buffer;
            /*  this.address = (double*)Marshal.AllocHGlobal(this.buffer.Length * sizeof(double));
              for (int i = 0; i < this.buffer.Length; i++)
              {
                   this.address[i] = this.buffer[i];
              }*/
            this.Set_A_quaternion(mq);
        }

        /// Constructor that builds a rotation matrix from an gale of rotation and an axis,
        /// defined in absolute coords. NOTE, axis must be normalized!
        public ChMatrix33(double angle,             //< angle of rotation, in radians
                          ChVector axis) : this()  //< axis of rotation, normalized
        {
            this.Set_A_AngAxis(angle, axis);
        }

        /// Multiplies this ChMatrix33 matrix and another ChMatrix33 matrix.
        /// Performance warning: a new object is created.
        public static ChMatrix33<double> operator *(ChMatrix33<Real> matbis, ChMatrix33<Real> matbis2)
        {
            ChMatrix33<double> result = new ChMatrix33<double>(0);
            result.nm.matrix.MatrMultiply(matbis.nm.matrix, matbis2.nm.matrix);
            return result;
        }

        public static bool operator ==(ChMatrix33<Real> op1, ChMatrix33<Real> op2)
        {
            return op1.Equals(op2);
        }

        public static bool operator !=(ChMatrix33<Real> op1, ChMatrix33<Real> op2)
        {
            return !op1.Equals(op2);
        }

        /// Sets the rotation matrix from an gale of rotation and an axis,
        /// defined in _absolute_ coords. NOTE, axis must be normalized!
        public void Set_A_AngAxis(double angle,            //< angle of rotation, in radians
                                  ChVector axis)  //< axis of rotation, normalized
        {
            ChQuaternion mr = new ChQuaternion(0, 0, 0, 0);// ChQuaternion.QNULL;

            mr.Q_from_AngAxis(angle, axis);
            this.Set_A_quaternion(mr);
        }

        /// Given a 3x3 rotation matrix, computes the corresponding
        /// quaternion.
        public ChQuaternion Get_A_quaternion()
        {
            ChQuaternion q = new ChQuaternion(0, 0, 0, 0); //ChQuaternion.QNULL;
            double s, tr;
            //dynamic a = 0.5;
            double half = 0.5;
            //dynamic h = half;

            // for speed reasons: ..
            double m00 = this.nm.matrix.Get33Element(0, 0);
            double m01 = this.nm.matrix.Get33Element(0, 1);
            double m02 = this.nm.matrix.Get33Element(0, 2);
            double m10 = this.nm.matrix.Get33Element(1, 0);
            double m11 = this.nm.matrix.Get33Element(1, 1);
            double m12 = this.nm.matrix.Get33Element(1, 2);
            double m20 = this.nm.matrix.Get33Element(2, 0);
            double m21 = this.nm.matrix.Get33Element(2, 1);
            double m22 = this.nm.matrix.Get33Element(2, 2);


            tr = m00 + m11 + m22;  // diag sum
           // dynamic tr2 = tr;

            if (tr >= 0)
            {
                s = Math.Sqrt(tr + 1);
                //dynamic s2 = s;
                q.e0 = half * s;
                s = half / s;
                q.e1 = (m21 - m12) * s;
                q.e2 = (m02 - m20) * s;
                q.e3 = (m10 - m01) * s;
            }
            else
            {
                int i = 0;

                if (m11 > m00)
                {
                    i = 1;
                    if (m22 > m11)
                        i = 2;
                }
                else
                {
                    if (m22 > m00)
                        i = 2;
                }

                switch (i)
                {
                    case 0:
                        s = Math.Sqrt(m00 - m11 - m22 + 1);
                        q.e1 = half * s;
                        s = half / s;
                        q.e2 = (m01 + m10) * s;
                        q.e3 = (m20 + m02) * s;
                        q.e0 = (m21 - m12) * s;
                        break;
                    case 1:
                        s = Math.Sqrt(m11 - m22 - m00 + 1);
                        q.e2 = half * s;
                        s = half / s;
                        q.e3 = (m12 + m21) * s;
                        q.e1 = (m01 + m10) * s;
                        q.e0 = (m02 - m20) * s;
                        break;
                    case 2:
                        s = Math.Sqrt(m22 - m00 - m11 + 1);
                        q.e3 = half * s;
                        s = half / s;
                        q.e1 = (m20 + m02) * s;
                        q.e2 = (m12 + m21) * s;
                        q.e0 = (m10 - m01) * s;
                        break;
                }
            }

            return q;
        }


        /// Complete generic constructor from 9 elements, ordered as three in first row, 
        /// three in second row, three in third row.
        public ChMatrix33(double m00, double m01, double m02,
                          double m10, double m11, double m12,
                          double m20, double m21, double m22) : this()
        {
            this.nm.matrix.Set33Element(0, 0, m00);
            this.nm.matrix.Set33Element(0, 1, m01);
            this.nm.matrix.Set33Element(0, 2, m02);
            this.nm.matrix.Set33Element(1, 0, m10);
            this.nm.matrix.Set33Element(1, 1, m11);
            this.nm.matrix.Set33Element(1, 2, m12);
            this.nm.matrix.Set33Element(2, 0, m20);
            this.nm.matrix.Set33Element(2, 1, m21);
            this.nm.matrix.Set33Element(2, 2, m22);
        }

        //
        // OPERATORS
        //

        /// Multiplies this matrix by a vector.
        public static ChVector operator *(ChMatrix33<Real> a, ChVector myvect) { return a.Matr_x_Vect(myvect); }


        //
        // FUNCTIONS
        //

        /// Reset to identity a 3x3 matrix (ones on diagonal, zero elsewhere)
        /// Note: optimized, for 3x3 matrices ONLY!
        public void Set33Identity()
        {
            nm = new ChMatrixNM<IntInterface.Three, IntInterface.Three>(0);
           // nm.matrix = new ChMatrix();
            this.nm.matrix.Reset();
            this.nm.matrix.Set33Element(0, 0, 1);
            this.nm.matrix.Set33Element(1, 1, 1);
            this.nm.matrix.Set33Element(2, 2, 1);
        }

        /// Multiplies this matrix (transposed) by a vector, as [M]'*v
        ///  \return The result of the multiplication, i.e. a vector.
        public ChVector MatrT_x_Vect(ChVector va)
        {
            return new ChVector(this.nm.matrix.Get33Element(0, 0) * va.x + this.nm.matrix.Get33Element(1, 0) * va.y +
                                      this.nm.matrix.Get33Element(2, 0) * va.z,
                                  this.nm.matrix.Get33Element(0, 1) * va.x + this.nm.matrix.Get33Element(1, 1) * va.y +
                                      this.nm.matrix.Get33Element(2, 1) * va.z,
                                  this.nm.matrix.Get33Element(0, 2) * va.x + this.nm.matrix.Get33Element(1, 2) * va.y +
                                      this.nm.matrix.Get33Element(2, 2) * va.z);
        }

        /// Fast inversion of small matrices. Result will be in 'matra'.
        /// \return Returns the determinant.
        public double FastInvert(ChMatrix33<Real> matra)
        {
            double det;
            double sdet0, sdet1, sdet2;

            sdet0 = +(this.nm.matrix.Get33Element(1, 1) * this.nm.matrix.Get33Element(2, 2)) -
                    (this.nm.matrix.Get33Element(2, 1) * this.nm.matrix.Get33Element(1, 2));
            sdet1 = -(this.nm.matrix.Get33Element(1, 0) * this.nm.matrix.Get33Element(2, 2)) +
                    (this.nm.matrix.Get33Element(2, 0) * this.nm.matrix.Get33Element(1, 2));
            sdet2 = +(this.nm.matrix.Get33Element(1, 0) * this.nm.matrix.Get33Element(2, 1)) -
                    (this.nm.matrix.Get33Element(2, 0) * this.nm.matrix.Get33Element(1, 1));

            det = sdet0 * this.nm.matrix.Get33Element(0, 0) + sdet1 * this.nm.matrix.Get33Element(0, 1) + sdet2 * this.nm.matrix.Get33Element(0, 2);

            matra.nm.matrix.Set33Element(0, 0, sdet0 / det);
            matra.nm.matrix.Set33Element(1, 0, sdet1 / det);
            matra.nm.matrix.Set33Element(2, 0, sdet2 / det);
            matra.nm.matrix.Set33Element(0, 1, (-(this.nm.matrix.Get33Element(0, 1) * this.nm.matrix.Get33Element(2, 2)) +
                                      (this.nm.matrix.Get33Element(2, 1) * this.nm.matrix.Get33Element(0, 2))) /
                                         det);
            matra.nm.matrix.Set33Element(1, 1, (+(this.nm.matrix.Get33Element(0, 0) * this.nm.matrix.Get33Element(2, 2)) -
                                      (this.nm.matrix.Get33Element(2, 0) * this.nm.matrix.Get33Element(0, 2))) /
                                         det);
            matra.nm.matrix.Set33Element(2, 1, (-(this.nm.matrix.Get33Element(0, 0) * this.nm.matrix.Get33Element(2, 1)) +
                                      (this.nm.matrix.Get33Element(2, 0) * this.nm.matrix.Get33Element(0, 1))) /
                                         det);
            matra.nm.matrix.Set33Element(0, 2, (+(this.nm.matrix.Get33Element(0, 1) * this.nm.matrix.Get33Element(1, 2)) -
                                      (this.nm.matrix.Get33Element(1, 1) * this.nm.matrix.Get33Element(0, 2))) /
                                         det);
            matra.nm.matrix.Set33Element(1, 2, (-(this.nm.matrix.Get33Element(0, 0) * this.nm.matrix.Get33Element(1, 2)) +
                                      (this.nm.matrix.Get33Element(1, 0) * this.nm.matrix.Get33Element(0, 2))) /
                                         det);
            matra.nm.matrix.Set33Element(2, 2, (+(this.nm.matrix.Get33Element(0, 0) * this.nm.matrix.Get33Element(1, 1)) -
                                      (this.nm.matrix.Get33Element(1, 0) * this.nm.matrix.Get33Element(0, 1))) /
                                         det);

            return det;
        }

        /// Fills a 3x3 matrix as a rotation matrix, given the three
        /// angles of consecutive rotations about x,y,z axis.
        public void Set_A_Rxyz(ChVector xyz)
        {
            double cx = Math.Cos(xyz.x);
            double cy = Math.Cos(xyz.y);
            double cz = Math.Cos(xyz.z);
            double sx = Math.Sin(xyz.x);
            double sy = Math.Sin(xyz.y);
            double sz = Math.Sin(xyz.z);

            this.nm.matrix.Set33Element(0, 0, (cy * cz));
            this.nm.matrix.Set33Element(0, 1, (cy * sz));
            this.nm.matrix.Set33Element(0, 2, (-sy));
            this.nm.matrix.Set33Element(1, 0, ((sx * sy * cz) - (cx * sz)));
            this.nm.matrix.Set33Element(1, 1, ((sx * sy * sz) + (cx * cz)));
            this.nm.matrix.Set33Element(1, 2, (sx * cy));
            this.nm.matrix.Set33Element(2, 0, ((cx * sy * cz) + (sx * sz)));
            this.nm.matrix.Set33Element(2, 1, ((cx * sy * sz) - (sx * cz)));
            this.nm.matrix.Set33Element(2, 2, (cx * cy));
        }


    /// Given a 3x3 rotation matrix, returns the versor of X axis.
    public ChVector Get_A_Xaxis() {
            ChVector X = new ChVector(0, 0, 0);// ChVector.VNULL;
            X.x = this.nm.matrix.Get33Element(0, 0);
            X.y = this.nm.matrix.Get33Element(1, 0);
            X.z = this.nm.matrix.Get33Element(2, 0);
            return X;
        }

        /// Given a 3x3 rotation matrix, returns the versor of Y axis.
        public ChVector Get_A_Yaxis() {
            ChVector Y = new ChVector(0, 0, 0);// ChVector.VNULL;
            Y.x = this.nm.matrix.Get33Element(0, 1);
            Y.y = this.nm.matrix.Get33Element(1, 1);
            Y.z = this.nm.matrix.Get33Element(2, 1);
            return Y;
        }

        /// Given a 3x3 rotation matrix, returns the versor of Z axis.
        public ChVector Get_A_Zaxis()
        {
            ChVector Z = new ChVector(0, 0, 0); //ChVector.VNULL;
            Z.x = this.nm.matrix.Get33Element(0, 2);
            Z.y = this.nm.matrix.Get33Element(1, 2);
            Z.z = this.nm.matrix.Get33Element(2, 2);
            return Z;
        }

    /// Multiplies this matrix by a vector, like in coordinate rotation [M]*v.
    ///  \return The result of the multiplication, i.e. a vector.    
    public ChVector Matr_x_Vect(ChVector va)
        {
            return new ChVector(this.nm.matrix.Get33Element(0, 0) * va.x + this.nm.matrix.Get33Element(0, 1) * va.y +
                                      this.nm.matrix.Get33Element(0, 2) * va.z,
                                this.nm.matrix.Get33Element(1, 0) * va.x + this.nm.matrix.Get33Element(1, 1) * va.y +
                                      this.nm.matrix.Get33Element(1, 2) * va.z,
                                this.nm.matrix.Get33Element(2, 0) * va.x + this.nm.matrix.Get33Element(2, 1) * va.y +
                                      this.nm.matrix.Get33Element(2, 2) * va.z);
        }

        /// Fills a 3x3 matrix as a rotation matrix corresponding
        /// to the rotation expressed by the quaternion 'quat'.
        public void Set_A_quaternion(ChQuaternion quat)
        {
            double e0e0 = (quat.e0 * quat.e0);
            double e1e1 = (quat.e1 * quat.e1);
            double e2e2 = (quat.e2 * quat.e2);
            double e3e3 = (quat.e3 * quat.e3);
            double e0e1 = (quat.e0 * quat.e1);
            double e0e2 = (quat.e0 * quat.e2);
            double e0e3 = (quat.e0 * quat.e3);
            double e1e2 = (quat.e1 * quat.e2);
            double e1e3 = (quat.e1 * quat.e3);
            double e2e3 = (quat.e2 * quat.e3);

            this.nm.matrix.Set33Element(0, 0, (e0e0 + e1e1) * 2 - 1);
            this.nm.matrix.Set33Element(0, 1, (e1e2 - e0e3) * 2);
            this.nm.matrix.Set33Element(0, 2, (e1e3 + e0e2) * 2);
            this.nm.matrix.Set33Element(1, 0, (e1e2 + e0e3) * 2);
            this.nm.matrix.Set33Element(1, 1, (e0e0 + e2e2) * 2 - 1);
            this.nm.matrix.Set33Element(1, 2, (e2e3 - e0e1) * 2);
            this.nm.matrix.Set33Element(2, 0, (e1e3 - e0e2) * 2);
            this.nm.matrix.Set33Element(2, 1, (e2e3 + e0e1) * 2);
            this.nm.matrix.Set33Element(2, 2, (e0e0 + e3e3) * 2 - 1);
        }

        /// Fills a 3x3 matrix as the "star" matrix, representing vector cross product.
        /// That is, given two 3d vectors a and b, aXb= [Astar]*b
        public void Set_X_matrix(ChVector vect)
        {
            this.nm.matrix.Set33Element(0, 0, 0);
            this.nm.matrix.Set33Element(0, 1, -vect.z);
            this.nm.matrix.Set33Element(0, 2, vect.y);
            this.nm.matrix.Set33Element(1, 0, vect.z);
            this.nm.matrix.Set33Element(1, 1, 0);
            this.nm.matrix.Set33Element(1, 2, -vect.x);
            this.nm.matrix.Set33Element(2, 0, -vect.y);
            this.nm.matrix.Set33Element(2, 1, vect.x);
            this.nm.matrix.Set33Element(2, 2, 0);
        }

        /// Fills a 3x3 matrix as a rotation matrix, given the three
        /// versors X,Y,Z of the basis.
        public void Set_A_axis(ChVector X, ChVector Y, ChVector Z)
        {
            this.nm.matrix.Set33Element(0, 0, X.x);
            this.nm.matrix.Set33Element(0, 1, Y.x);
            this.nm.matrix.Set33Element(0, 2, Z.x);
            this.nm.matrix.Set33Element(1, 0, X.y);
            this.nm.matrix.Set33Element(1, 1, Y.y);
            this.nm.matrix.Set33Element(1, 2, Z.y);
            this.nm.matrix.Set33Element(2, 0, X.z);
            this.nm.matrix.Set33Element(2, 1, Y.z);
            this.nm.matrix.Set33Element(2, 2, Z.z);
        }

        /// Return true if this matrix is the identity 3x3 matrix.
        public bool IsIdentity()
        {
            return this.nm.matrix.Get33Element(0, 0) == 1 && this.nm.matrix.Get33Element(0, 1) == 0 && this.nm.matrix.Get33Element(0, 2) == 0 &&
                   this.nm.matrix.Get33Element(1, 0) == 0 && this.nm.matrix.Get33Element(1, 1) == 1 && this.nm.matrix.Get33Element(1, 2) == 0 &&
                   this.nm.matrix.Get33Element(2, 0) == 0 && this.nm.matrix.Get33Element(2, 1) == 0 && this.nm.matrix.Get33Element(2, 2) == 1;
        }

}
}
