import * as React from 'react';
import { ErrorMessage, Form, Formik } from 'formik';
import TextInput from '../../app/common/form/TextInput';
import { Button, Header, Label } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';
import * as Yup from 'yup';

function RegisterForm() {
    const { userStore } = useStore();

    return (
        <Formik
            initialValues={{ displayName: '', username: '', email: '', password: '' }}
            onSubmit={(values, { setErrors }) => userStore.register(values).catch(error =>
                setErrors({ password: 'Invalid password', email: 'invalid email' }))}
            validationSchema={Yup.object({
                displayName: Yup.string().required(),
                username: Yup.string().required(),
                email: Yup.string().required().email(),
                password: Yup.string().required(),
            })}
        >
            {
                ({ handleSubmit, isSubmitting, errors,isValid,dirty }) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <Header as='h2' textAlign='center' content='Signup to Reactivities' color='teal' />
                        <TextInput name='displayName' placeholder='Display Name' />
                        <TextInput name='username' placeholder='Username' />
                        <TextInput name='email' placeholder='Email' />
                        <TextInput name='password' placeholder='Password' type='password' />
                        <ErrorMessage name="error" render={() => <Label style={{ marginBottom: 10 }} basic color='red' content={errors.email} />} />
                        <Button disabled={!isValid || !dirty || isSubmitting}
                            loading={isSubmitting} positive content='Register' type='submit' fluid />
                    </Form>
                )
            }
        </Formik>
    );
}

export default observer(RegisterForm);